using System;
using System.Collections.Generic;

namespace LogForRestartSr
{
    public class RestartSrException : Exception
    {
        public enum ExceptionCode
        {
            // 正常
            //Err_0 = 0,

            // 連接錯誤
            Err_97 = 97,

            // 指定IP下對應服务不存在
            Err_98 = 98,

            // 預期外
            Err_99 = 99
        };

        private static readonly Dictionary<ExceptionCode, string> _messages =
            new Dictionary<ExceptionCode, string>
            {
                { ExceptionCode.Err_97, "連接錯誤"} ,
                { ExceptionCode.Err_98, "指定IP下對應服务不存在" },
                { ExceptionCode.Err_99, "預期外錯誤"}
            };

        public ExceptionCode ExceptionCd { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RestartSrException()
            : base()
        {
            // 普通のコンストラクタ
        }

        /// <summary>
        /// メッセージ文字列を渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        public RestartSrException(ExceptionCode exceptionCode)
            : base(_messages[exceptionCode])
        {
            ExceptionCd = exceptionCode;
        }

        /// <summary>
        /// メッセージコードと発生済み例外オブジェクトを渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージコード</param>
        /// <param name="innerException">発生済み例外オブジェクト</param>
        public RestartSrException(ExceptionCode exceptionCode, Exception innerException)
            : base(_messages[exceptionCode], innerException)
        {
            ExceptionCd = exceptionCode;
        }
    }
}
