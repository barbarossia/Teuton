using BaiduNetDisk.DataContracts;
using BaiduNetDisk.ExceptionHandler;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;
using Utility.Web;

namespace BaiduNetDisk
{
    public class BaiduClient
    {
        public string AccessToken { get; set; }
        private const string BaseUrl = "https://pcs.baidu.com/rest/2.0/pcs/";
        private const string AdvanceUrl = "https://pcs.baidu.com/rest/2.0/pcs/services/";
        private const string ServiceUrl = "http://pan.baidu.com/rest/2.0/services/";

        public BaiduClient():this(null)
        {
        }

        public BaiduClient(string accessToken)
        {
            AccessToken = accessToken;
        }

        public NetDiskInfo GetDiskInfo()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("quota?method={method}&access_token={access_token}", Method.GET);
            request.AddParameter("method", "info", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken, ParameterType.UrlSegment);

            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetDiskInfo();
        }

        public NetFileInfo UploadSingleFile(string sourcePath, string destPath)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("file?method={method}&path={path}&access_token={access_token}", Method.POST);
            request.AddParameter("method", "upload", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken, ParameterType.UrlSegment);
            request.AddParameter("path", destPath, ParameterType.UrlSegment);

            request.AddFile(sourcePath.GetFileName(), sourcePath);
            request.AlwaysMultipartFormData = true;

            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetFileDetail();
        }

        public NetTorrenInfo QueryTorrent(string path)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "query_sinfo", ParameterType.UrlSegment);

            request.AddParameter("source_path", path);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("type", "2");
            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTorrenInfo();
        }

        public List<NetTaskInfo> QueryTaskList()
        {
            var client = new RestClient(AdvanceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "list_task", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("need_task_info", "1");
            request.AddParameter("status", 255);
            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTaskList();

        }

        public List<NetTaskInfo> QueryTaskDetail(List<NetTaskInfo> tasks)
        {
            var client = new RestClient(AdvanceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "query_task", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("task_ids", string.Join(",", tasks.Select(t => t.Id)));
            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTaskDetail();
        }

        public NetTaskInfo AddTorrentTask(string sourcePath, string destPath, NetTorrenInfo info)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "add_task", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("source_path", sourcePath);
            request.AddParameter("file_sha1", info.SHA1);
            request.AddParameter("save_path", destPath);
            request.AddParameter("type", 2);
            request.AddParameter("selected_idx", info.Files.Aggregate("", (output, item) => string.Format("{0},{1}", output, info.Files.IndexOf(item))));
            request.AddParameter("task_from", 1);

            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTaskInfo();
        }

        public NetTaskInfo AddTask(string url, string destPath)
        {
            var client = new RestClient(ServiceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "add_task", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("save_path", destPath);
            request.AddParameter("source_url", url);

            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTaskInfo();
        }

        public NetTaskInfo CancelTask(long taskId)
        {
            var client = new RestClient(AdvanceUrl);
            var request = new RestRequest("cloud_dl?method={method}", Method.POST);

            request.AddParameter("method", "cancel_task", ParameterType.UrlSegment);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("task_id", taskId.ToString());

            var response = client.Execute(request);
            HandleResponseExcepition(response);
            return response.Content.ToNetTaskInfo();
        }

        private void HandleResponseExcepition(IRestResponse response)
        {
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new WebProtocolException(HttpStatusCode.BadRequest, message, response.ErrorException);
            }
            else
            {
                CheckStatusCode(response);
            }
        }

        private void CheckStatusCode(IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    CheckServiceError(response.Content);
                    return;
                case HttpStatusCode.BadRequest:
                    CheckServiceError(response.Content);
                    break;
                case HttpStatusCode.Forbidden:
                    CheckServiceError(response.Content);
                    break;
                case HttpStatusCode.InternalServerError:
                    throw new WebProtocolException(response.StatusCode,
                        response.Content, null);
                case HttpStatusCode.Unauthorized:
                    throw new WebProtocolException(response.StatusCode,
                        string.Format("You have no permission to access {0}.", response.ResponseUri), null);
                default:
                    throw new WebProtocolException(response.StatusCode,
                        string.Format("\"{0}\" returned from {1}.", response.StatusCode, response.ResponseUri), null);
            }
        }

        private void CheckServiceError(string json)
        {
            ServiceError error = json.ToServiceError();
            switch (error.ErrorCode)
            {
                case 31061:
                    throw new ServiceFileAlreadyExistsException(error.ErrorCode, error.Message, error.RequestId);
                case 36022:
                    throw new ServiceAddTaskException(error.ErrorCode, error.Message, error.RequestId);
                case 36018:
                    throw new ServiceInvalidTorrent(error.ErrorCode, error.Message, error.RequestId);
                case 36004:
                    throw new ServiceInvalidTorrent(error.ErrorCode, error.Message, error.RequestId);
            }
        }
    }
}
