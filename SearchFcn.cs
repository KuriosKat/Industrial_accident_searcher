using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Net;
using Unity.VisualScripting;

public class SearchFcn : MonoBehaviour
{
    //입력함수와 관련된 변수
    public InputField InputFieldCompanyName;
    public Button Button;

    //의무보고 2회이상 미흡 사업장 변수 
    public Text CompanyStatus;
    public Text CompanyStatus_over2cnt;

    //동종업 평균재해율 이상 사업장 변수
    public Text AvgDisasterRate;
    public Text AvgDisasterRate_Mancount; //재해자 수 (중대재해자 포함)
    public Text AvgDisasterRate_importantMancount; //중대재해자 수

    //동종업 평균사망만인율 이상 사업장 변수
    public Text AvgDeathRate;
    public Text AvgDeathRate_Mancount;
    public Text AvgDeathRate_importantMancount;

    //private string companyArr;
    private string company = "test";
    int danger_flag = 0;
    //사진 이벤트
    public Image InitImage;
    public Sprite ChangeSprite_Safe;
    public Sprite ChangeSprite_Danger;
    public void Change_Image_Danger()
    {
        InitImage.sprite = ChangeSprite_Danger;
        Debug.Log("Image_changed_by_Danger");
    }
    public void Change_Image_Safe()
    {
        InitImage.sprite = ChangeSprite_Safe;
        Debug.Log("Image_changed_by_Safe");
    }
    public void ComTextInputAndButtonClick() //이게 사실상 Main문
    {
        /*
        if(InputFieldCompanyName.text == company) //test입력후 버튼 클릭시 이벤트
        {
            Debug.Log("company activated");
            OpenApiData_2OUT();
        }*/
        string CompanyName = InputFieldCompanyName.text;

        OpenApiData_2OUT(CompanyName);
        OpenApiData_avgDisaster(CompanyName);
        OpenApiData_avgDeath(CompanyName);
        Invoke("ImageEvent", 1); //쿼리가 늦는 경우 async라 비동기가 있어 1초 지연
    }
    async void OpenApiData_2OUT(string CName) //보고의무 2회 이상 위반 사업장
    {
        HttpClient client = new HttpClient(); //clinet 객체 생성

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.odcloud.kr/api/15090007/v1/uddi:304106fa-3815-418e-8bf1-8e653031d182?page=1&perPage=1000");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Authorization", "Infuser bPan3uT7M6moW38sPmui+aQQIvG5LOWQFp7yfWWh8TURYp/k69yfkKnBCCXMIKaINNGLoBMdxgHt8WlWLIAuiQ==");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        // Debug.Log(responseBody[10]);
        JObject jObject = JObject.Parse(responseBody);

        string count = JsonConvert.SerializeObject(jObject["currentCount"], Formatting.Indented);
        int cnt = int.Parse(count);
        string[] stringArray = new string[cnt];

        for(int i=0; i<cnt; i++)
        {
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["사업장명"], Formatting.Indented);//json을 string으로 캐스팅 변환
            stringArray[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        
        if (!(string.IsNullOrWhiteSpace(CName))) //공백, 다중 입력은 예외처리하여 무시
        {
            for(int i=0; i<cnt; i++)
            {
                if (stringArray[i].Contains(CName)) //[공공데이터API] 명단 회사명중에 검색어 함유 문자열 있는가 -> "ex) 좋아건설"에 "좋아"만 있어도 True
                {
                    //Debug.Log("둠황차");
                    //Debug.Log(jObject["data"][i]["위반횟수"]);
                    CompanyStatus.text = "[산재 발생 보고의무 준수도]: 경고";
                    CompanyStatus_over2cnt.text = "위반횟수: " + JsonConvert.SerializeObject(jObject["data"][i]["위반횟수"], Formatting.Indented) + "회";
                    ++danger_flag;
                    break;
                }
                else //명단에 회사 이름이 없으면
                {
                    // Debug.Log("취업성공");
                    CompanyStatus.text = "[산재 발생 보고의무 준수도]: 안전";
                    CompanyStatus_over2cnt.text = "";
                }
            }
        }
        Debug.Log(danger_flag);
    }

    async void OpenApiData_avgDisaster(string CName)
    {
        HttpClient client = new HttpClient(); //clinet 객체 생성

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.odcloud.kr/api/15090150/v1/uddi:ed2e8b69-e6d1-4a3c-99c5-c72dff0c0e36?page=1&perPage=1000");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Authorization", "Infuser bPan3uT7M6moW38sPmui+aQQIvG5LOWQFp7yfWWh8TURYp/k69yfkKnBCCXMIKaINNGLoBMdxgHt8WlWLIAuiQ==");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        // Debug.Log(responseBody[10]);
        JObject jObject = JObject.Parse(responseBody);

        string count = JsonConvert.SerializeObject(jObject["currentCount"], Formatting.Indented);
        int cnt = int.Parse(count);
        string[] stringArray_avgDisaster = new string[cnt];

        for (int i = 0; i < cnt; i++)
        {
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["사업장명(현장명)"], Formatting.Indented);//json을 string으로 캐스팅 변환
            stringArray_avgDisaster[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        if (!(string.IsNullOrWhiteSpace(CName))) //공백, 다중 입력은 예외처리하여 무시
        {
            for (int i = 0; i < cnt; i++)
            {
                if (stringArray_avgDisaster[i].Contains(CName)) //[공공데이터API] 명단 회사명중에 검색어 함유 문자열 있는가 -> "ex) 좋아건설"에 "좋아"만 있어도 True
                {
                    //Debug.Log("둠황차");
                    //Debug.Log(jObject["data"][i]["위반횟수"]);
                    AvgDisasterRate.text = "[동종업종 평균재해율 이상여부]: 경고 " + JsonConvert.SerializeObject(jObject["data"][i]["재해율"], Formatting.Indented);
                    AvgDisasterRate_Mancount.text = "재해자 수(중대재해자 포함): " + JsonConvert.SerializeObject(jObject["data"][i]["재해자수(명)"], Formatting.Indented) + "명";
                    AvgDisasterRate_importantMancount.text = "중대재해자 수: " + JsonConvert.SerializeObject(jObject["data"][i]["중대재해자수(명)"], Formatting.Indented) + "명";
                    ++danger_flag;
                    break;
                }
                else //명단에 회사 이름이 없으면
                {
                    // Debug.Log("취업성공");
                    AvgDisasterRate.text = "[동종업종 평균재해율 이상여부]: 안전 ";
                    AvgDisasterRate_Mancount.text = "";
                    AvgDisasterRate_importantMancount.text = "";
                }
            }
        }
        Debug.Log(danger_flag);
    }
    async void OpenApiData_avgDeath(string CName) //평균사망만인률 부분
    {
        HttpClient client = new HttpClient(); //clinet 객체 생성

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.odcloud.kr/api/15090126/v1/uddi:62de7bba-ab05-4d8d-876e-ab398dee3486?page=1&perPage=1000");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Authorization", "Infuser bPan3uT7M6moW38sPmui+aQQIvG5LOWQFp7yfWWh8TURYp/k69yfkKnBCCXMIKaINNGLoBMdxgHt8WlWLIAuiQ==");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        // Debug.Log(responseBody[10]);
        JObject jObject = JObject.Parse(responseBody);

        string count = JsonConvert.SerializeObject(jObject["currentCount"], Formatting.Indented);
        int cnt = int.Parse(count);
        string[] stringArray_avgDisaster = new string[cnt];

        for (int i = 0; i < cnt; i++)
        {
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["사업장명"], Formatting.Indented);//json을 string으로 캐스팅 변환
            stringArray_avgDisaster[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        if (!(string.IsNullOrWhiteSpace(CName))) //공백, 다중 입력은 예외처리하여 무시
        {
            for (int i = 0; i < cnt; i++)
            {
                if (stringArray_avgDisaster[i].Contains(CName)) //[공공데이터API] 명단 회사명중에 검색어 함유 문자열 있는가 -> "ex) 좋아건설"에 "좋아"만 있어도 True
                {
                    //Debug.Log("둠황차");
                    //Debug.Log(jObject["data"][i]["위반횟수"]);
                    AvgDeathRate.text = "[동종업종 평균사망만인율]: 경고 ";
                    AvgDeathRate_Mancount.text = "사망만인율:  " + JsonConvert.SerializeObject(jObject["data"][i]["사망만인율"], Formatting.Indented) + "%";
                    AvgDeathRate_importantMancount.text = "사망자수(명): " + JsonConvert.SerializeObject(jObject["data"][i]["사망자수(명)"], Formatting.Indented) + "명";
                    ++danger_flag;
                    break;
                }
                else //명단에 회사 이름이 없으면
                {
                    // Debug.Log("취업성공");
                    AvgDeathRate.text = "[동종업종 평균사망만인율 이상여부]: 안전 ";
                    AvgDeathRate_Mancount.text = "";
                    AvgDeathRate_importantMancount.text = "";
                }
            }
        }
        Debug.Log(danger_flag);
    }
    void ImageEvent()
    {
        Debug.Log("ImageEvent: " + danger_flag);
        if (danger_flag == 0) //안전한곳, 이미지 이벤트처리
        {
            Change_Image_Safe();
        }
        else //위험한곳, 이미지 이벤트처리
        {
            Change_Image_Danger();
            danger_flag = 0;
        }
        danger_flag = 0;
    }
}
