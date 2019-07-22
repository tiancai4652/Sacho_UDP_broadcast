using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BroadcastLibrary
{
    /// <summary>
    /// 广播消息解析器
    /// </summary>
    public class BoardcastMsgAnalysis
    {
        /// <summary>
        /// 消息处理器字典
        /// </summary>
        public static Dictionary<string, Func<string, string>> msgHandlers = new Dictionary<string, Func<string, string>>();

        static BoardcastMsgAnalysis()
        {
            msgHandlers.Add("GetACalCoreService", GetACalCoreService);//获取服务地址
        }

        /// <summary>
        /// 获取回复消息
        /// </summary>
        /// <param name="buffer">接收的消息</param>
        /// <returns></returns>
        public static string GetReply(byte[] data)
        {
            string msg = System.Text.Encoding.UTF8.GetString(data);
            string[] values = msg.Split(new[] { Global.MsgSplitter }, StringSplitOptions.None);
            string msgType = values[0];
            if (!msgHandlers.ContainsKey(msgType))
                return string.Empty;

            Func<string, string> handler = msgHandlers[msgType];
            string replay = handler(values.Length > 1 ? values[1] : string.Empty);
            return $"{values[0]}{ Global.MsgSplitter}{replay}";
        }

        #region 消息处理器

        /// <summary>
        /// /获取服务地址
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static string GetACalCoreService(string parameter)
        {
            return "T04," + DateTime.Now.ToLongTimeString();
        }

        #endregion
    }
}
