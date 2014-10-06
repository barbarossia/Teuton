using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduNetDisk.DataContracts
{
    public class NetTaskInfo
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public decimal Rate { get; set; }
        public int Status { get; set; }//(0下载成功，1下载进行中 2系统错误，3资源不存在，4下载超时，5资源存在但下载失败 6存储空间不足 7目标地址数据已存在 8任务取消)
        public DateTime CreateTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string Name { get; set; }
        public int Rapidownload { get; set; }
        public long TotalSize { get; set; }
        public long FinishedSize { get; set; }
        public int Result { get; set; }//0查询成功，结果有效，1要查询的task_id不存在
        public List<NetFileInfo> Files { get; set; }
        public long RequestId { get; set; }
    }
}
