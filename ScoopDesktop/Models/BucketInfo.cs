namespace ScoopDesktop.Models;

public class BucketInfo
{
    public string BucketName { get; private set; }
    public string BucketDir { get; set; }
    public string[] AppList { get; set; }

    public BucketInfo(string bucketName, string bucketDir, string[] appList)
    {
        BucketName = bucketName;
        BucketDir = bucketDir;
        AppList = appList;
    }
}
