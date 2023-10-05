using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SMO.Service.AD;
using System.Linq;
using System.Dynamic;

namespace SMO.Service.MD
{
    public class VendorVehicleService : GenericService<T_MD_VENDOR_VEHICLE, VendorVehicleRepo>
    {
        public T_MD_VEHICLE ObjVehicle { get; set; } 
        public bool IsInList { get; set; }
        public VendorVehicleService() : base()
        {
            IsInList = false;
            ObjVehicle = new T_MD_VEHICLE();
        }

        public void CheckVehicle()
        {
            try
            {
                //Chống hack
                //var userService = new UserService();
                //dynamic param = new ExpandoObject();
                //param.IsFetch_ListUserVendor = true;
                //userService.Get(ProfileUtilities.User.USER_NAME, param);
                //if (userService.ObjDetail.ListUserVendor.Count(x => x.VENDOR_CODE == this.ObjDetail.VENDOR_CODE) == 0)
                //{
                //    this.State = false;
                //    return;
                //}

                //if (!UnitOfWork.Repository<VehicleRepo>().CheckExist(x => x.CODE == this.ObjDetail.VEHICLE_CODE))
                //{
                //    this.SynchronizeVehicle();
                //}

                //this.ObjVehicle = UnitOfWork.Repository<VehicleRepo>().Get(this.ObjDetail.VEHICLE_CODE);
                //if (this.ObjVehicle != null)
                //{
                //    var lstComparment = this.ObjVehicle.ListCompartment.Count();

                //    if (this.CurrentRepository.CheckExist(x => x.VENDOR_CODE == this.ObjDetail.VENDOR_CODE && x.VEHICLE_CODE == this.ObjDetail.VEHICLE_CODE))
                //    {
                //        this.IsInList = true;
                //    }

                //}
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public void GetListVehicleVT()
        {
            //Chống hack
            //var userService = new UserService();
            //dynamic param = new ExpandoObject();
            //param.IsFetch_ListUserVendor = true;
            //userService.Get(ProfileUtilities.User.USER_NAME, param);
            //if (userService.ObjDetail.ListUserVendor.Count(x => x.VENDOR_CODE == this.ObjDetail.VENDOR_CODE) == 0)
            //{
            //    this.State = false;
            //    return;
            //}

            this.Search();
        }

        public void AddToListVT(string vehicleCode, string vendorCode)
        {
            try
            {
                ////Chống hack
                //var userService = new UserService();
                //dynamic param = new ExpandoObject();
                //param.IsFetch_ListUserVendor = true;
                //userService.Get(ProfileUtilities.User.USER_NAME, param);
                //if (userService.ObjDetail.ListUserVendor.Count(x => x.VENDOR_CODE == vendorCode) == 0)
                //{
                //    this.State = false;
                //    return;
                //}

                //if (this.CurrentRepository.CheckExist(x => x.VENDOR_CODE == vendorCode && x.VEHICLE_CODE == vehicleCode))
                //{
                //    this.ErrorMessage = "Phương tiện này đã thuộc danh sách quản lý.";
                //    this.State = false;
                //    return;
                //}

                //if (!UnitOfWork.Repository<VehicleRepo>().CheckExist(x => x.CODE == vehicleCode))
                //{
                //    this.ErrorMessage = "Không tồn tại phương tiện này!";
                //    this.State = false;
                //    return;
                //}

                //this.ObjVehicle = UnitOfWork.Repository<VehicleRepo>().Get(vehicleCode);

                //UnitOfWork.BeginTransaction();

                //this.CurrentRepository.Create(new T_MD_VENDOR_VEHICLE()
                //{
                //    VENDOR_CODE = vendorCode,
                //    VEHICLE_CODE = vehicleCode,
                //    CREATE_BY = ProfileUtilities.User.USER_NAME
                //});

                //if (!this.ObjVehicle.ACTIVE)
                //{
                //    this.ObjVehicle.ACTIVE = true;
                //    UnitOfWork.Repository<VehicleRepo>().Update(this.ObjVehicle);
                //}

                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void SynchronizeVehicle()
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new SynMD_Vehicle_Function();
                    if (systemConfig.ObjDetail.LAST_SYN_VEHICLE.HasValue)
                    {
                        functionSAP.Parameters = new
                        {
                            I_VEHICLE = this.ObjDetail.VEHICLE_CODE
                        };
                    }

                    var result = conn.ExecuteFunction(functionSAP).ToList();
                    if (result.Count == 0)
                    {
                        return;
                    }

                    var lstVehicle = from x in result
                                     group x by new { x.VEHICLE_CODE, x.TRANSUNIT_CODE, x.UNIT, x.TRANSMODE_CODE, x.OIC_PBATCH, x.OIC_PTRIP } into grp
                                     select new
                                     {
                                         CODE = grp.Key.VEHICLE_CODE,
                                         TRANSUNIT_CODE = grp.Key.TRANSUNIT_CODE,
                                         TRANSMODE_CODE = grp.Key.TRANSMODE_CODE,
                                         UNIT = grp.Key.UNIT,
                                         OIC_PBATCH = grp.Key.OIC_PBATCH,
                                         OIC_PTRIP = grp.Key.OIC_PTRIP,
                                         CAPACITY = grp.Sum(x => x.CAPACITY)
                                     }
                                     ;
                    lstVehicle = lstVehicle.Distinct();

                    var lstCompartment = result.Select(x => new T_MD_VEHICLE_COMPARTMENT
                    {
                        CODE = x.VEHICLE_CODE + x.SEQ_NUMBER,
                        VEHICLE_CODE = x.VEHICLE_CODE,
                        SEQ_NUMBER = x.SEQ_NUMBER,
                        CAPACITY = x.CAPACITY
                    });

                    UnitOfWork.BeginTransaction();

                    foreach (var item in lstVehicle)
                    {
                        var vehicle = new T_MD_VEHICLE()
                        {
                            CODE = item.CODE,
                            TRANSUNIT_CODE = item.TRANSUNIT_CODE,
                            TRANSMODE_CODE = item.TRANSMODE_CODE,
                            OIC_PBATCH = item.OIC_PBATCH,
                            OIC_PTRIP = item.OIC_PTRIP,
                            UNIT = item.UNIT,
                            CAPACITY = item.CAPACITY,
                            ACTIVE = true,
                            CREATE_BY = ProfileUtilities.User.USER_NAME
                        };
                        UnitOfWork.Repository<VehicleRepo>().Create(vehicle);
                    }

                    foreach (var item in lstCompartment)
                    {
                        var comparment = new T_MD_VEHICLE_COMPARTMENT()
                        {
                            CODE = item.CODE,
                            VEHICLE_CODE = item.VEHICLE_CODE,
                            SEQ_NUMBER = item.SEQ_NUMBER,
                            CAPACITY = item.CAPACITY,
                            CREATE_BY = ProfileUtilities.User.USER_NAME
                        };

                        UnitOfWork.Repository<VehicleCompartmentRepo>().Create(comparment);
                    }

                    UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
