[System.Serializable]
class DetectParams
{
    private string _imagePath;

    public string ImagePath
    {
        get { return _imagePath; }
        set { _imagePath = value; }
    }

    private string _imageBase64;

    public string ImageBase64
    {
        get { return _imageBase64; }
        set { _imageBase64 = value; }
    }

}