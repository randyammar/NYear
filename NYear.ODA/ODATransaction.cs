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
        private event ODATransactionEventHandler _Committing;
        private event ODATransactionEventHandler _RollBacking;

        internal event ODATransactionEventHandler Committing
        {
            add
            {
                if (_Committing != null)
                {
                    Delegate[] dls = _Committing.GetInvocationList();
                    foreach (Delegate dl in dls)
                        if (dl.Method == value.Method)
                            return;
                }
                _Committing += value;
            }
            remove
            {
                if (_Committing != null)
                    _Committing -= value;
            }
        }
        internal event ODATransactionEventHandler RollBacking
        {
            add
            {
                if (_RollBacking != null)
                {
                    Delegate[] dls = _RollBacking.GetInvocationList();
                    foreach (Delegate dl in dls)
                        if (dl.Method == value.Method)
                            return;
                }
                _RollBacking += value;
            }
            remove
            {
                if (_RollBacking != null)
                    _RollBacking -= value;
            }
        }

        internal ODATransaction(int TimeOut)
        {
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
            DisposeTimer();
            if (_Committing == null)
                return;
            _Committing();
            _Committing = null;
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            DisposeTimer();
            if (_RollBacking == null)
                return;
            _RollBacking();
            _RollBacking = null;
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
