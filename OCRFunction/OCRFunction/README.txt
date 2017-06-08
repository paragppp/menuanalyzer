Please add appsettings.json to the project with the following content:
If creating directly on server, add the below keys to the application keys inside Azure Function

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "AzureWebJobsDashboard": "",
    "accessKeyName": "Ocp-Apim-Subscription-Key",
    "OcrKeyValue": "YOUR VISION API KEY",
    "OcrURL": "https://<server location>.api.cognitive.microsoft.com/vision/v1.0/ocr?language=unk&detectOrientation=true",
    "imageSearchURL": "https://api.cognitive.microsoft.com/bing/v5.0/images/search?q=",
    "imageSearchKeyValue": "YOUR BING SEARCH API KEY"
  }
}
