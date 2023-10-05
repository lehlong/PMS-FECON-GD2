using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PROJECT_STRUCT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid? PARENT_ID { get; set; }
        public virtual Guid? BOQ_ID { get; set; }
        public virtual Guid? WBS_ID { get; set; }
        public virtual Guid? ACTIVITY_ID { get; set; }
        public virtual Guid? TASK_ID { get; set; }
        public virtual string UNIT_CODE { get; set; }
        public virtual string GEN_CODE { get; set; }
        public virtual string TEXT { get; set; }
        public virtual string STATUS { get; set; }
        public virtual double C_ORDER { get; set; }
        public virtual string TYPE { get; set; }
        public virtual decimal? QUANTITY { get; set; }
        public virtual decimal? PRICE { get; set; }
        public virtual decimal? TOTAL { get; set; }
        /// <summary>
        /// Khối lượng thiết kế
        /// </summary>
        public virtual decimal? PLAN_VOLUME { get; set; }
        public virtual DateTime START_DATE { get; set; }
        public virtual DateTime FINISH_DATE { get; set; }
        public virtual bool IS_CREATE_ON_SAP { get; set; }
        public virtual string NguoiPhuTrach { get; set; }
        private T_PS_PROJECT_STRUCT _Parent;
        public virtual T_PS_PROJECT_STRUCT Parent
        {
            get
            {
                try
                {
                    if (this._Parent != null)
                    {
                        var text = this._Parent.ID;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Parent;
            }
            set
            {
                this._Parent = value;
            }
        }

        public virtual T_PS_PROJECT Project { get; set; }
        public virtual T_PS_WBS Wbs { get; set; }
        public virtual T_PS_BOQ Boq { get; set; }
        public virtual T_PS_ACTIVITY Activity { get; set; }
        public virtual T_PS_CONTRACT_DETAIL ContractDetail { get; set; }

        private T_MD_UNIT _Unit;
        public virtual T_MD_UNIT Unit
        {
            get
            {
                try
                {
                    if (this._Unit != null)
                    {
                        var text = this._Unit.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Unit;
            }
            set
            {
                this._Unit = value;
            }
        }

    }
}
