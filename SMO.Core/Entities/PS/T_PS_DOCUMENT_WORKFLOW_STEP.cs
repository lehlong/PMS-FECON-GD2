using SMO.Core.Entities.MD;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_DOCUMENT_WORKFLOW_STEP : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string WORKFLOW_CODE { get; set; }
        public virtual Guid WORKFLOW_ID { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PROJECT_ROLE_CODE { get; set; }

        public virtual string USER_ACTION { get; set; }

        public virtual int NUMBER_DAYS { get; set; }
        public virtual string ACTION { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool IS_DONE { get; set; }
        public virtual bool IS_PROCESS { get; set; }
        public virtual string SOLVED { get; set; }
        public virtual Guid DOCUMENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }

        private T_MD_PROJECT_ROLE _ProjectRole;
        public virtual T_MD_PROJECT_ROLE ProjectRole
        {
            get
            {
                try
                {
                    if (this._ProjectRole != null)
                    {
                        var text = this._ProjectRole.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._ProjectRole;
            }
            set
            {
                this._ProjectRole = value;
            }
        }
        private T_AD_USER _UserAction;
        public virtual T_AD_USER UserAction
        {
            get
            {
                try
                {
                    if (this._UserAction != null)
                    {
                        var text = this._UserAction.FULL_NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._UserAction;
            }
            set
            {
                this._UserAction = value;
            }
        }

        private T_MD_ACTION_WORKFLOW _Solved;
        public virtual T_MD_ACTION_WORKFLOW Solved
        {
            get
            {
                try
                {
                    if (this._Solved != null)
                    {
                        var text = this._Solved.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Solved;
            }
            set
            {
                this._Solved = value;
            }
        }
    }
}