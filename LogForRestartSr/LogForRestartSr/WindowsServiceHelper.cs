using System;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace LogForRestartSr
{
    public class WindowsServiceHelper
    {

        private string ServerAddress;

        private string User;

        private string Password;

        private string ServiceNm;

        private string ServiceInterval;

        private string ProgramStartupType;

        public WindowsServiceHelper(string ServerAddress, string User, string Password, string ServiceNm, string ServiceInterval, string ProgramStartupType)
        {

            this.ServerAddress = ServerAddress.Replace("\\", "");
            this.User = User;
            this.Password = Password;
            this.ServiceNm = ServiceNm;
            this.ServiceInterval = ServiceInterval;
            this.ProgramStartupType = ProgramStartupType;
        }

        /// <summary>
        /// 验证是否能连接到远程计算机
        /// </summary>
        /// <param name="host">远程IP</param>
        /// <param name="userName">用戶名</param>
        /// <param name="password">連接密碼</param>
        /// <returns>bool</returns>
        private bool RemoteConnectValidate()
        {
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Username = User;
            connectionOptions.Password = Password;
            ManagementScope managementScope = new ManagementScope("//" + ServerAddress + "/root/cimv2", connectionOptions);
            try
            {
                managementScope.Connect();
                return managementScope.IsConnected;
            }
            catch (Exception ex)
            {
                // 连接錯誤
                throw new RestartSrException(RestartSrException.ExceptionCode.Err_97, ex);
            }
        }

        /// <summary>
        /// 获取windows 服务状态
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serverIP">服务器IP</param>
        /// <returns></returns>
        private ServiceControllerStatus GetServiceState(string serviceName, string serverIP)
        {
            try
            {
                ServiceControllerStatus serviceSate;
                using (ServiceController sc = ServiceController.GetServices(serverIP).FirstOrDefault(x => x.ServiceName == serviceName))
                {
                    if (sc == null)
                    {
                        // 指定IP下對應服务不存在
                        throw new RestartSrException(RestartSrException.ExceptionCode.Err_98);
                    }
                    serviceSate = sc.Status;
                    sc.Close();
                }
                return serviceSate;
            }
            catch (RestartSrException rsException)
            {
                // 指定IP下對應服务不存在
                throw rsException;
            }
            catch (Exception ex)
            {
                // 預期外錯誤
                throw new RestartSrException(RestartSrException.ExceptionCode.Err_99, ex);
            }
        }

        /// <summary>
        /// 停止或启动Windows 服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serverIP">远程IP</param>
        /// <param name="stop">是否是停止</param>
        private void StopAndStartWindowsService(string serviceName, string serverIP, bool stop)
        {
            try
            {
                using (ServiceController sc = ServiceController.GetServices(serverIP)
                .FirstOrDefault(x => x.ServiceName == serviceName))
                {
                    if (sc == null)
                    {
                        // 指定IP下對應服务不存在
                        throw new RestartSrException(RestartSrException.ExceptionCode.Err_98);
                    }

                    StopAndStartWindowsService(sc, stop);

                    //等待启动
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    sc.Refresh();

                    sc.Close();
                }
            }
            catch (Exception ex)
            {
                // 預期外錯誤
                throw new RestartSrException(RestartSrException.ExceptionCode.Err_99, ex);
            }
        }

        /// <summary>
        /// 停止或启动Windows 服务
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="stop"></param>
        private void StopAndStartWindowsService(ServiceController sc, bool stop = true)
        {
            Action act = () =>
            {
                ServiceControllerStatus serviceSate = sc.Status;
                if (stop && (serviceSate == ServiceControllerStatus.StartPending || serviceSate == ServiceControllerStatus.Running))
                {
                    //如果当前应用程序池是启动或者正在启动状态，调用停止方法
                    sc.Stop();

                    // //等待停止
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    sc.Refresh();
                }

                serviceSate = sc.Status;
                if (serviceSate == ServiceControllerStatus.Stopped || serviceSate == ServiceControllerStatus.StopPending)
                {
                    sc.Start();
                }
            };

            try
            {
                //  執行 停止或启动Windows 服务
                act();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 停止或启动Windows 服务
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="stop"></param>
        public void ExeStartService()
        {
            try
            {
                if ("localhost".Equals(this.ProgramStartupType.ToLower()))
                {
                    if (RemoteConnectValidate())
                    {
                        // 获取windows 服务状态
                        //ServiceControllerStatus serviceSate = GetServiceState(ServiceNm, ServerAddress);

                        // 停止或启动Windows 服务
                        StopAndStartWindowsService(ServiceNm, ServerAddress, true);
                        //}
                    }
                    else
                    {
                        // 連接錯誤
                        throw new RestartSrException(RestartSrException.ExceptionCode.Err_97);
                    }
                }
                else if ("server".Equals(this.ProgramStartupType.ToLower()))
                {

                    // 获取windows 服务状态
                    ServiceControllerStatus serviceSate = GetServiceState(ServiceNm, ServerAddress);

                    // 停止或启动Windows 服务
                    StopAndStartWindowsService(ServiceNm, ServerAddress, true);

                }

            }
            catch (RestartSrException rsException)
            {
                throw rsException;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
