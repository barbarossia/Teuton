using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Web;
using Utility.Common;
using System.Diagnostics;

namespace BaiduNetDisk.DataContracts
{
    public static class TypeConversionExtension
    {
        public static NetDiskInfo ToNetDiskInfo(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);
            return new NetDiskInfo()
            {
                TotalSize = (string)content["quota"],
                UsedSize = (string)content["used"],
            };
        }

        public static NetFileInfo ToNetFileDetail(this string json)
        {
            JsonValue result = System.Json.JsonObject.Parse(json);

            return new NetFileInfo()
            {
                Id = (string)result["fs_id"],
                Path = ((string)result["path"]).UrlDecode(),
                Size = (long)result["size"],
                CreateDate = ((long)(result["ctime"])).ToDateTime(),
                ModifyDate = ((long)(result["mtime"])).ToDateTime(),
                MD5 = (string)result["md5"]
            };
        }

        public static NetTorrenInfo ToNetTorrenInfo(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);
            var torrent = content["torrent_info"];

            return new NetTorrenInfo()
            {
                SHA1 = (string)torrent["sha1"],
                FileCount = (int)torrent["file_count"],
                Files = torrent.ContainsKey("file_info") ? torrent["file_info"].ToNetFileInfo() : null,
            };
        }

        public static List<NetFileInfo> ToNetFileListInfo(this JsonValue files)
        {
            return files.Select(f => new NetFileInfo()
            {
                Name = ((string)f.Value["file_name"]).UrlDecode(),
                Size = (long)f.Value["file_size"],
            }).ToList();
        }

        public static List<NetFileInfo> ToNetFileInfo(this JsonValue files)
        {
            return files.Select(f => new NetFileInfo()
            {
                Name = ((string)f.Value["file_name"]).UrlDecode(),
                Size = (long)f.Value["size"],
            }).ToList();
        }

        public static List<NetTaskInfo> ToNetTaskList(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);

            return content["task_info"].Select(task => new NetTaskInfo()
                {
                    Id = (long)task.Value["task_id"],
                    Name = (string)task.Value["task_name"],
                    Url = ((string)task.Value["source_url"]).UrlDecode(),
                    Path = ((string)task.Value["save_path"]).UrlDecode(),
                    Rate = (decimal)task.Value["rate_limit"],
                    Status = (int)task.Value["status"],
                    CreateTime = ((long)(task.Value["create_time"])).ToDateTime(),
                }).ToList();
        }

        public static List<NetTaskInfo> ToNetTaskDetail(this string json)
        {
            //Debug.WriteLine(json);
            JsonValue content = System.Json.JsonObject.Parse(json);

            return content["task_info"].Select(task => new NetTaskInfo()
            {
                Name = task.Value.ContainsKey("task_name") ? ((string)task.Value["task_name"]).UrlDecode() : string.Empty,
                Url = task.Value.ContainsKey("source_url") ? ((string)task.Value["source_url"]).UrlDecode() : string.Empty,
                Path = task.Value.ContainsKey("save_path") ? ((string)task.Value["save_path"]).UrlDecode() : string.Empty,
                Status = task.Value.ContainsKey("status") ? (int)task.Value["status"] : 0,
                CreateTime = task.Value.ContainsKey("create_time") ? ((long)(task.Value["create_time"])).ToDateTime(): 0L.ToDateTime(),
                StartTime = task.Value.ContainsKey("start_time") ? ((long)(task.Value["start_time"])).ToDateTime(): 0L.ToDateTime(),
                FinishTime = task.Value.ContainsKey("finish_time") ? ((long)(task.Value["finish_time"])).ToDateTime(): 0L.ToDateTime(),
                TotalSize = task.Value.ContainsKey("file_size") ? (long)task.Value["file_size"] : 0L,
                FinishedSize = task.Value.ContainsKey("finished_size") ? (long)task.Value["finished_size"] : 0L,
                Files = task.Value.ContainsKey("file_list") ? ToNetFileListInfo(task.Value["file_list"]) : null,
            }).ToList();
        }


        public static NetTaskInfo ToNetTaskInfo(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);
            return new NetTaskInfo()
            {
                Id = content.ContainsKey("task_id") ? (long)content["task_id"] : 0L,
                Rapidownload = content.ContainsKey("rapid_download") ? (int)content["rapid_download"] : 0,
                RequestId = (long)content["request_id"],
            };
        }

        public static string ToMD5(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);
            return (string)content["md5"];
        }

        public static ServiceError ToServiceError(this string json)
        {
            JsonValue content = System.Json.JsonObject.Parse(json);
            return new ServiceError()
            {
                ErrorCode = (long)content["error_code"],
                Message = (string)content["error_msg"],
                RequestId = (long)content["request_id"],
            };
        }
    }
}
