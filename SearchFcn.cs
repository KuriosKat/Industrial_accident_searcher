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
    //�Է��Լ��� ���õ� ����
    public InputField InputFieldCompanyName;
    public Button Button;

    //�ǹ����� 2ȸ�̻� ���� ����� ���� 
    public Text CompanyStatus;
    public Text CompanyStatus_over2cnt;

    //������ ��������� �̻� ����� ����
    public Text AvgDisasterRate;
    public Text AvgDisasterRate_Mancount; //������ �� (�ߴ������� ����)
    public Text AvgDisasterRate_importantMancount; //�ߴ������� ��

    //������ ��ջ�������� �̻� ����� ����
    public Text AvgDeathRate;
    public Text AvgDeathRate_Mancount;
    public Text AvgDeathRate_importantMancount;

    //private string companyArr;
    private string company = "test";
    int danger_flag = 0;
    //���� �̺�Ʈ
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
    public void ComTextInputAndButtonClick() //�̰� ��ǻ� Main��
    {
        /*
        if(InputFieldCompanyName.text == company) //test�Է��� ��ư Ŭ���� �̺�Ʈ
        {
            Debug.Log("company activated");
            OpenApiData_2OUT();
        }*/
        string CompanyName = InputFieldCompanyName.text;

        OpenApiData_2OUT(CompanyName);
        OpenApiData_avgDisaster(CompanyName);
        OpenApiData_avgDeath(CompanyName);
        Invoke("ImageEvent", 1); //������ �ʴ� ��� async�� �񵿱Ⱑ �־� 1�� ����
    }
    async void OpenApiData_2OUT(string CName) //�����ǹ� 2ȸ �̻� ���� �����
    {
        HttpClient client = new HttpClient(); //clinet ��ü ����

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
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["������"], Formatting.Indented);//json�� string���� ĳ���� ��ȯ
            stringArray[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        
        if (!(string.IsNullOrWhiteSpace(CName))) //����, ���� �Է��� ����ó���Ͽ� ����
        {
            for(int i=0; i<cnt; i++)
            {
                if (stringArray[i].Contains(CName)) //[����������API] ��� ȸ����߿� �˻��� ���� ���ڿ� �ִ°� -> "ex) ���ưǼ�"�� "����"�� �־ True
                {
                    //Debug.Log("��Ȳ��");
                    //Debug.Log(jObject["data"][i]["����Ƚ��"]);
                    CompanyStatus.text = "[���� �߻� �����ǹ� �ؼ���]: ���";
                    CompanyStatus_over2cnt.text = "����Ƚ��: " + JsonConvert.SerializeObject(jObject["data"][i]["����Ƚ��"], Formatting.Indented) + "ȸ";
                    ++danger_flag;
                    break;
                }
                else //��ܿ� ȸ�� �̸��� ������
                {
                    // Debug.Log("�������");
                    CompanyStatus.text = "[���� �߻� �����ǹ� �ؼ���]: ����";
                    CompanyStatus_over2cnt.text = "";
                }
            }
        }
        Debug.Log(danger_flag);
    }

    async void OpenApiData_avgDisaster(string CName)
    {
        HttpClient client = new HttpClient(); //clinet ��ü ����

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
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["������(�����)"], Formatting.Indented);//json�� string���� ĳ���� ��ȯ
            stringArray_avgDisaster[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        if (!(string.IsNullOrWhiteSpace(CName))) //����, ���� �Է��� ����ó���Ͽ� ����
        {
            for (int i = 0; i < cnt; i++)
            {
                if (stringArray_avgDisaster[i].Contains(CName)) //[����������API] ��� ȸ����߿� �˻��� ���� ���ڿ� �ִ°� -> "ex) ���ưǼ�"�� "����"�� �־ True
                {
                    //Debug.Log("��Ȳ��");
                    //Debug.Log(jObject["data"][i]["����Ƚ��"]);
                    AvgDisasterRate.text = "[�������� ��������� �̻󿩺�]: ��� " + JsonConvert.SerializeObject(jObject["data"][i]["������"], Formatting.Indented);
                    AvgDisasterRate_Mancount.text = "������ ��(�ߴ������� ����): " + JsonConvert.SerializeObject(jObject["data"][i]["�����ڼ�(��)"], Formatting.Indented) + "��";
                    AvgDisasterRate_importantMancount.text = "�ߴ������� ��: " + JsonConvert.SerializeObject(jObject["data"][i]["�ߴ������ڼ�(��)"], Formatting.Indented) + "��";
                    ++danger_flag;
                    break;
                }
                else //��ܿ� ȸ�� �̸��� ������
                {
                    // Debug.Log("�������");
                    AvgDisasterRate.text = "[�������� ��������� �̻󿩺�]: ���� ";
                    AvgDisasterRate_Mancount.text = "";
                    AvgDisasterRate_importantMancount.text = "";
                }
            }
        }
        Debug.Log(danger_flag);
    }
    async void OpenApiData_avgDeath(string CName) //��ջ�����η� �κ�
    {
        HttpClient client = new HttpClient(); //clinet ��ü ����

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
            string SearchedCompanyName = JsonConvert.SerializeObject(jObject["data"][i]["������"], Formatting.Indented);//json�� string���� ĳ���� ��ȯ
            stringArray_avgDisaster[i] = SearchedCompanyName;
            // Debug.Log(stringArray[i]);
        }
        if (!(string.IsNullOrWhiteSpace(CName))) //����, ���� �Է��� ����ó���Ͽ� ����
        {
            for (int i = 0; i < cnt; i++)
            {
                if (stringArray_avgDisaster[i].Contains(CName)) //[����������API] ��� ȸ����߿� �˻��� ���� ���ڿ� �ִ°� -> "ex) ���ưǼ�"�� "����"�� �־ True
                {
                    //Debug.Log("��Ȳ��");
                    //Debug.Log(jObject["data"][i]["����Ƚ��"]);
                    AvgDeathRate.text = "[�������� ��ջ��������]: ��� ";
                    AvgDeathRate_Mancount.text = "���������:  " + JsonConvert.SerializeObject(jObject["data"][i]["���������"], Formatting.Indented) + "%";
                    AvgDeathRate_importantMancount.text = "����ڼ�(��): " + JsonConvert.SerializeObject(jObject["data"][i]["����ڼ�(��)"], Formatting.Indented) + "��";
                    ++danger_flag;
                    break;
                }
                else //��ܿ� ȸ�� �̸��� ������
                {
                    // Debug.Log("�������");
                    AvgDeathRate.text = "[�������� ��ջ�������� �̻󿩺�]: ���� ";
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
        if (danger_flag == 0) //�����Ѱ�, �̹��� �̺�Ʈó��
        {
            Change_Image_Safe();
        }
        else //�����Ѱ�, �̹��� �̺�Ʈó��
        {
            Change_Image_Danger();
            danger_flag = 0;
        }
        danger_flag = 0;
    }
}
