namespace Covid19Radar.Services.Logs
{
    public interface ILogPathDependencyService
    {
        string LogsDirPath { get; }
        string LogUploadingTmpPath { get; }
        string LogUploadingPublicPath { get; }
    }
}
