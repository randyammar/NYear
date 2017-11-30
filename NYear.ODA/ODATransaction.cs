/* 
 * 2.分布式事务很难做而且性能不理想，应用服务器暂时做不到，所以在应用服务上不能有跨库事务。
 * 目前的分布式事务解决方案是二阶段提交（PreCommit、doCommit）或三阶段提交（CanCommit、PreCommit、doCommit）。
 * 跨库事务一般都是数据库层面考虑。
*/
using System;

namespace NYear.ODA
{
    internal delegate void ODATransactionEventHandler();
    /// <summary>
    /// ODA事务
    /// </summary>
    public class ODATransaction
    {
        private System.Timers.Timer Tim = null;
        private string _TransactionId = null;
        private event ODATransactionEventHandler _DoCommit;
        private event ODATransactionEventHandler _DoRollBack;

        internal event ODATransactionEventHandler CanCommit;
        internal event ODATransactionEventHandler PreCommit;
        internal event ODATransactionEventHandler DoCommit
        {
            add
            {
                if (_DoCommit != null)
                {
                    Delegate[] dls = _DoCommit.GetInvocationList();
                    foreach (Delegate dl in dls)
                        if (dl.Method == value.Method)
                            return;
                }
                _DoCommit += value;
            }
            remove
            {
                if (_DoCommit != null)
                    _DoCommit -= value;
            }
        }
        internal event ODATransactionEventHandler RollBacking
        {
            add
            {
                if (_DoRollBack != null)
                {
                    Delegate[] dls = _DoRollBack.GetInvocationList();
                    foreach (Delegate dl in dls)
                        if (dl.Method == value.Method)
                            return;
                }
                _DoRollBack += value;
            }
            remove
            {
                if (_DoRollBack != null)
                    _DoRollBack -= value;
            }
        }
        public string TransactionId { get { return _TransactionId; } }
        internal ODATransaction(int TimeOut)
        {
            _TransactionId = Guid.NewGuid().ToString("N");
            Tim = new System.Timers.Timer(TimeOut * 1000);
            Tim.Elapsed += new System.Timers.ElapsedEventHandler(Tim_Elapsed);
            Tim.Start();
        }
        private void Tim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RollBack();
        }
        
       
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            //分布式事务，二阶段提交，或三阶段提交
            //暂不支持
            //try
            //{
            //    if (CanCommit == null)
            //        CanCommit();
            //    if (PreCommit == null)
            //        PreCommit();
            //}
            //catch
            //{
            //    RollBack();
            //}
            DisposeTimer();
            if (_DoCommit == null)
                return;
            _DoCommit();
            _DoCommit = null;
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            DisposeTimer();
            if (_DoRollBack == null)
                return;
            _DoRollBack();
            _DoRollBack = null;
        }
        private void DisposeTimer()
        {
            if (Tim != null)
            {
                if (Tim.Enabled)
                    Tim.Stop();
                Tim.Close();
                Tim.Dispose();
                Tim = null;
            }
        }
    }
}
