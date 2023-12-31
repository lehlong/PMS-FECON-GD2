USE [PMS_UAT]
GO
/****** Object:  View [dbo].[V_WORK_VOLUME_PERIOD]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_WORK_VOLUME_PERIOD]
AS
SELECT        ID AS VolumeWorkId, PROJECT_ID AS ProjectId,
                             (SELECT        TOP (1) ID
                               FROM            dbo.T_PS_TIME AS t
                               WHERE        (dbo.T_PS_VOLUME_WORK.TO_DATE <= FINISH_DATE) AND (dbo.T_PS_VOLUME_WORK.TO_DATE >= START_DATE) AND (PROJECT_ID = dbo.T_PS_VOLUME_WORK.PROJECT_ID)) AS TimeId
FROM            dbo.T_PS_VOLUME_WORK
WHERE        (STATUS = '05')
GO
/****** Object:  View [dbo].[V_GTSL_TH_BOQ_TOTAL]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_TH_BOQ_TOTAL]
AS
SELECT        dbo.V_WORK_VOLUME_PERIOD.ProjectId, dbo.V_WORK_VOLUME_PERIOD.TimeId, SUM(dbo.T_PS_VOLUME_WORK_DETAIL.VALUE * dbo.T_PS_PROJECT_STRUCT.PRICE) AS GTSL_TH_BOQ
FROM            dbo.T_PS_VOLUME_WORK_DETAIL INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_VOLUME_WORK_DETAIL.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID INNER JOIN
                         dbo.V_WORK_VOLUME_PERIOD ON dbo.T_PS_VOLUME_WORK_DETAIL.HEADER_ID = dbo.V_WORK_VOLUME_PERIOD.VolumeWorkId
GROUP BY dbo.V_WORK_VOLUME_PERIOD.ProjectId, dbo.V_WORK_VOLUME_PERIOD.TimeId
GO
/****** Object:  View [dbo].[V_ACCEPT_VOLUME_PERIOD]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_ACCEPT_VOLUME_PERIOD]
AS
SELECT        ID AS VolumeAcceptId, PROJECT_ID AS ProjectId,
                             (SELECT        ID
                               FROM            dbo.T_PS_TIME AS t
                               WHERE        (dbo.T_PS_VOLUME_ACCEPT.TO_DATE <= FINISH_DATE) AND (dbo.T_PS_VOLUME_ACCEPT.TO_DATE >= START_DATE) AND (PROJECT_ID = dbo.T_PS_VOLUME_ACCEPT.PROJECT_ID)) AS TimeId
FROM            dbo.T_PS_VOLUME_ACCEPT
WHERE        (STATUS = '05')
GO
/****** Object:  View [dbo].[V_GTSL_NGHIEM_THU_BOQ_TOTAL]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_NGHIEM_THU_BOQ_TOTAL]
AS
SELECT        dbo.V_ACCEPT_VOLUME_PERIOD.ProjectId, dbo.V_ACCEPT_VOLUME_PERIOD.TimeId, SUM(dbo.T_PS_VOLUME_ACCEPT_DETAIL.VALUE * dbo.T_PS_PROJECT_STRUCT.PRICE) AS GTSL_NGHIEM_THU_BOQ
FROM            dbo.T_PS_VOLUME_ACCEPT_DETAIL INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_VOLUME_ACCEPT_DETAIL.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID INNER JOIN
                         dbo.V_ACCEPT_VOLUME_PERIOD ON dbo.T_PS_VOLUME_ACCEPT_DETAIL.HEADER_ID = dbo.V_ACCEPT_VOLUME_PERIOD.VolumeAcceptId
GROUP BY dbo.V_ACCEPT_VOLUME_PERIOD.ProjectId, dbo.V_ACCEPT_VOLUME_PERIOD.TimeId
GO
/****** Object:  View [dbo].[V_KY_BAO_CAO]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_KY_BAO_CAO]
AS
SELECT        dbo.T_PS_PROJECT.ID AS ProjectId, dbo.T_PS_TIME.ID AS TimeId, dbo.T_PS_TIME.START_DATE AS TU_NGAY, dbo.T_PS_TIME.FINISH_DATE AS DEN_NGAY, dbo.T_PS_TIME.C_ORDER + 1 AS KY_BC, 
                         dbo.T_PS_TIME.YEAR AS NAM
FROM            dbo.T_PS_PROJECT INNER JOIN
                         dbo.T_PS_TIME ON dbo.T_PS_PROJECT.ID = dbo.T_PS_TIME.PROJECT_ID
GO
/****** Object:  View [dbo].[V_TRANG_THAI_DA]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_TRANG_THAI_DA]
AS
SELECT        ID AS ProjectId, CASE STATUS WHEN '01' THEN N'Khởi tạo' WHEN '02' THEN N'Lập kế hoạch' WHEN '03' THEN N'Đang thực hiện' WHEN '04' THEN N'Đóng dự án' ELSE N'Khởi tạo' END AS TRANG_THAI_DA
FROM            dbo.T_PS_PROJECT
GO
/****** Object:  View [dbo].[V_CAP_DA]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_CAP_DA]
AS
SELECT        dbo.T_PS_PROJECT.ID AS ProjectId, dbo.T_MD_PROJECT_LEVEL.NAME AS CAP_DA
FROM            dbo.T_MD_PROJECT_LEVEL INNER JOIN
                         dbo.T_PS_PROJECT ON dbo.T_MD_PROJECT_LEVEL.CODE = dbo.T_PS_PROJECT.PROJECT_LEVEL_CODE
GO
/****** Object:  View [dbo].[V_HOP_DONG_CHINH]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_HOP_DONG_CHINH]
AS
SELECT        CONTRACT_NUMBER AS MA_HD, PROJECT_ID AS ProjectId
FROM            dbo.T_PS_CONTRACT
WHERE        (PARENT_CODE IS NULL) AND (IS_SIGN_WITH_CUSTOMER = 1)
GO
/****** Object:  View [dbo].[V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER]
AS
SELECT        SUM(dbo.T_PS_PLAN_COST.TOTAL) AS GTSL_KH_BOQ, dbo.T_PS_PLAN_COST.TIME_PERIOD_ID AS TimeId, dbo.T_PS_PROJECT_STRUCT.PROJECT_ID AS ProjectId
FROM            dbo.T_PS_PLAN_COST INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_PLAN_COST.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID
WHERE        (dbo.T_PS_PLAN_COST.IS_CUSTOMER = 1)
GROUP BY dbo.T_PS_PLAN_COST.TIME_PERIOD_ID, dbo.T_PS_PROJECT_STRUCT.PROJECT_ID
GO
/****** Object:  View [dbo].[BI_V_DS_CTDA_BOQ]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BI_V_DS_CTDA_BOQ]
AS
SELECT        dbo.V_KY_BAO_CAO.NAM, dbo.V_KY_BAO_CAO.TU_NGAY, dbo.V_KY_BAO_CAO.DEN_NGAY, dbo.V_KY_BAO_CAO.KY_BC, dbo.T_PS_PROJECT.CODE AS MA_DA, dbo.T_PS_PROJECT.NAME AS TEN_DA, dbo.V_CAP_DA.CAP_DA, 
                         dbo.V_HOP_DONG_CHINH.MA_HD, dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER.GTSL_KH_BOQ, dbo.V_GTSL_TH_BOQ_TOTAL.GTSL_TH_BOQ, 
                         dbo.V_GTSL_NGHIEM_THU_BOQ_TOTAL.GTSL_NGHIEM_THU_BOQ, dbo.V_TRANG_THAI_DA.TRANG_THAI_DA
FROM            dbo.V_CAP_DA LEFT OUTER JOIN
                         dbo.T_PS_PROJECT ON dbo.V_CAP_DA.ProjectId = dbo.T_PS_PROJECT.ID LEFT OUTER JOIN
                         dbo.V_TRANG_THAI_DA ON dbo.T_PS_PROJECT.ID = dbo.V_TRANG_THAI_DA.ProjectId LEFT OUTER JOIN
                         dbo.V_HOP_DONG_CHINH ON dbo.T_PS_PROJECT.ID = dbo.V_HOP_DONG_CHINH.ProjectId LEFT OUTER JOIN
                         dbo.V_KY_BAO_CAO ON dbo.T_PS_PROJECT.ID = dbo.V_KY_BAO_CAO.ProjectId LEFT OUTER JOIN
                         dbo.V_GTSL_TH_BOQ_TOTAL ON dbo.V_KY_BAO_CAO.ProjectId = dbo.V_GTSL_TH_BOQ_TOTAL.ProjectId AND dbo.V_KY_BAO_CAO.TimeId = dbo.V_GTSL_TH_BOQ_TOTAL.TimeId LEFT OUTER JOIN
                         dbo.V_GTSL_NGHIEM_THU_BOQ_TOTAL ON dbo.V_KY_BAO_CAO.ProjectId = dbo.V_GTSL_NGHIEM_THU_BOQ_TOTAL.ProjectId AND 
                         dbo.V_KY_BAO_CAO.TimeId = dbo.V_GTSL_NGHIEM_THU_BOQ_TOTAL.TimeId LEFT OUTER JOIN
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER ON dbo.V_KY_BAO_CAO.ProjectId = dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER.ProjectId AND 
                         dbo.V_KY_BAO_CAO.TimeId = dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER.TimeId
GO
/****** Object:  View [dbo].[V_LOAI_DA]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_LOAI_DA]
AS
SELECT        dbo.T_PS_PROJECT.ID AS ProjectId, dbo.T_MD_PROJECT_TYPE.NAME AS LOAI_DA
FROM            dbo.T_MD_PROJECT_TYPE INNER JOIN
                         dbo.T_PS_PROJECT ON dbo.T_MD_PROJECT_TYPE.CODE = dbo.T_PS_PROJECT.TYPE
GO
/****** Object:  View [dbo].[V_DV]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_DV]
AS
SELECT        dbo.T_PS_PROJECT.ID AS ProjectId, dbo.T_AD_ORGANIZE.COMPANY_CODE AS DV
FROM            dbo.T_PS_PROJECT INNER JOIN
                         dbo.T_AD_ORGANIZE ON dbo.T_PS_PROJECT.DON_VI = dbo.T_AD_ORGANIZE.PKID
WHERE        (dbo.T_AD_ORGANIZE.TYPE = N'CP')
GO
/****** Object:  View [dbo].[V_PB]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_PB]
AS
SELECT        dbo.T_PS_PROJECT.ID AS ProjectId, dbo.T_AD_ORGANIZE.NAME AS PB
FROM            dbo.T_PS_PROJECT INNER JOIN
                         dbo.T_AD_ORGANIZE ON dbo.T_PS_PROJECT.PHONG_BAN = dbo.T_AD_ORGANIZE.PKID
WHERE        (dbo.T_AD_ORGANIZE.TYPE = N'BP')
GO
/****** Object:  View [dbo].[V_HOP_DONG_KHACH_HANG]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_HOP_DONG_KHACH_HANG]
AS
SELECT        child.CONTRACT_NUMBER AS MA_HD, parent.CONTRACT_NUMBER AS MA_HDGOC, (CASE WHEN parent.CONTRACT_NUMBER IS NULL THEN N'HĐ' ELSE N'PLHĐ' END) AS TYPE, child.CONTRACT_VALUE AS GT_HD, 
                         child.PROJECT_ID AS ProjectId
FROM            dbo.T_PS_CONTRACT AS child LEFT OUTER JOIN
                         dbo.T_PS_CONTRACT AS parent ON child.PARENT_CODE = parent.ID
WHERE        (child.IS_SIGN_WITH_CUSTOMER = 1)
GO
/****** Object:  View [dbo].[BI_V_DS_CTDA_GTHD]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BI_V_DS_CTDA_GTHD]
AS
SELECT        YEAR(dbo.T_PS_PROJECT.START_DATE) AS NAM, dbo.T_PS_PROJECT.CODE AS MA_DA, dbo.T_PS_PROJECT.NAME AS TEN_DA, dbo.V_DV.DV, dbo.V_PB.PB, dbo.V_LOAI_DA.LOAI_DA, dbo.V_CAP_DA.CAP_DA, 
                         dbo.V_HOP_DONG_KHACH_HANG.MA_HD, dbo.V_HOP_DONG_KHACH_HANG.TYPE, dbo.V_HOP_DONG_KHACH_HANG.MA_HDGOC, dbo.V_HOP_DONG_KHACH_HANG.GT_HD
FROM            dbo.V_HOP_DONG_KHACH_HANG LEFT OUTER JOIN
                         dbo.T_PS_PROJECT ON dbo.V_HOP_DONG_KHACH_HANG.ProjectId = dbo.T_PS_PROJECT.ID LEFT OUTER JOIN
                         dbo.V_DV ON dbo.T_PS_PROJECT.ID = dbo.V_DV.ProjectId LEFT OUTER JOIN
                         dbo.V_PB ON dbo.T_PS_PROJECT.ID = dbo.V_PB.ProjectId LEFT OUTER JOIN
                         dbo.V_CAP_DA ON dbo.T_PS_PROJECT.ID = dbo.V_CAP_DA.ProjectId LEFT OUTER JOIN
                         dbo.V_LOAI_DA ON dbo.T_PS_PROJECT.ID = dbo.V_LOAI_DA.ProjectId
GO
/****** Object:  View [dbo].[V_GTSL_TH_COST_TOTAL]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_TH_COST_TOTAL]
AS
SELECT        dbo.V_WORK_VOLUME_PERIOD.ProjectId, dbo.V_WORK_VOLUME_PERIOD.TimeId, SUM(dbo.T_PS_VOLUME_WORK_DETAIL.VALUE * dbo.T_PS_PROJECT_STRUCT.PRICE) AS GTSL_TH, 
                         dbo.T_PS_PROJECT_STRUCT.ID AS StructureId
FROM            dbo.T_PS_VOLUME_WORK_DETAIL INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_VOLUME_WORK_DETAIL.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID INNER JOIN
                         dbo.V_WORK_VOLUME_PERIOD ON dbo.T_PS_VOLUME_WORK_DETAIL.HEADER_ID = dbo.V_WORK_VOLUME_PERIOD.VolumeWorkId
GROUP BY dbo.V_WORK_VOLUME_PERIOD.ProjectId, dbo.V_WORK_VOLUME_PERIOD.TimeId, dbo.T_PS_PROJECT_STRUCT.ID
GO
/****** Object:  View [dbo].[V_GTSL_NGHIEM_THU_COST_TOTAL]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_NGHIEM_THU_COST_TOTAL]
AS
SELECT        dbo.V_ACCEPT_VOLUME_PERIOD.ProjectId, dbo.V_ACCEPT_VOLUME_PERIOD.TimeId, SUM(dbo.T_PS_VOLUME_ACCEPT_DETAIL.VALUE * dbo.T_PS_PROJECT_STRUCT.PRICE) AS GTSL_NGHIEM_THU, 
                         dbo.T_PS_PROJECT_STRUCT.ID AS StructureId
FROM            dbo.T_PS_VOLUME_ACCEPT_DETAIL INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_VOLUME_ACCEPT_DETAIL.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID INNER JOIN
                         dbo.V_ACCEPT_VOLUME_PERIOD ON dbo.T_PS_VOLUME_ACCEPT_DETAIL.HEADER_ID = dbo.V_ACCEPT_VOLUME_PERIOD.VolumeAcceptId
GROUP BY dbo.V_ACCEPT_VOLUME_PERIOD.ProjectId, dbo.V_ACCEPT_VOLUME_PERIOD.TimeId, dbo.T_PS_PROJECT_STRUCT.ID
GO
/****** Object:  View [dbo].[V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR]
AS
SELECT        SUM(dbo.T_PS_PLAN_COST.TOTAL) AS TotalPlanCost, dbo.T_PS_PROJECT_STRUCT.ID AS StructureId, dbo.T_PS_PLAN_COST.TIME_PERIOD_ID AS TimeId, dbo.T_PS_PROJECT_STRUCT.PROJECT_ID AS ProjectId
FROM            dbo.T_PS_PLAN_COST INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.T_PS_PLAN_COST.PROJECT_STRUCT_ID = dbo.T_PS_PROJECT_STRUCT.ID
WHERE        (dbo.T_PS_PLAN_COST.IS_CUSTOMER = 0)
GROUP BY dbo.T_PS_PROJECT_STRUCT.ID, dbo.T_PS_PLAN_COST.TIME_PERIOD_ID, dbo.T_PS_PROJECT_STRUCT.PROJECT_ID
GO
/****** Object:  View [dbo].[V_HANG_MUC_CAY_CAU_TRUC_LV2]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_HANG_MUC_CAY_CAU_TRUC_LV2]
AS
SELECT        TEXT AS HANG_MUC, GEN_CODE AS MA_HANG_MUC, ID AS StructureId, PROJECT_ID AS ProjectId
FROM            dbo.T_PS_PROJECT_STRUCT AS ps
WHERE        (PARENT_ID =
                             (SELECT        TOP (1) ID
                               FROM            dbo.T_PS_PROJECT_STRUCT AS ps_parent
                               WHERE        (PARENT_ID IS NULL) AND (ps.PROJECT_ID = PROJECT_ID)))
GO
/****** Object:  View [dbo].[BI_V_DS_CTDA_CP]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BI_V_DS_CTDA_CP]
AS
SELECT        dbo.V_KY_BAO_CAO.NAM, dbo.V_KY_BAO_CAO.TU_NGAY, dbo.V_KY_BAO_CAO.DEN_NGAY, dbo.V_KY_BAO_CAO.KY_BC, dbo.T_PS_PROJECT.CODE AS MA_DA, dbo.T_PS_PROJECT.NAME AS TEN_DA, dbo.V_CAP_DA.CAP_DA, 
                         dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.HANG_MUC, dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.MA_HANG_MUC, dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR.TotalPlanCost AS GTSL_KH, 
                         dbo.V_GTSL_TH_COST_TOTAL.GTSL_TH, dbo.V_GTSL_NGHIEM_THU_COST_TOTAL.GTSL_NGHIEM_THU, dbo.V_TRANG_THAI_DA.TRANG_THAI_DA
FROM            dbo.V_GTSL_TH_COST_TOTAL RIGHT OUTER JOIN
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR RIGHT OUTER JOIN
                         dbo.V_GTSL_NGHIEM_THU_COST_TOTAL RIGHT OUTER JOIN
                         dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2 INNER JOIN
                         dbo.V_KY_BAO_CAO INNER JOIN
                         dbo.T_PS_PROJECT LEFT OUTER JOIN
                         dbo.V_CAP_DA ON dbo.T_PS_PROJECT.ID = dbo.V_CAP_DA.ProjectId INNER JOIN
                         dbo.V_TRANG_THAI_DA ON dbo.T_PS_PROJECT.ID = dbo.V_TRANG_THAI_DA.ProjectId ON dbo.V_KY_BAO_CAO.ProjectId = dbo.T_PS_PROJECT.ID ON 
                         dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.ProjectId = dbo.T_PS_PROJECT.ID ON dbo.V_GTSL_NGHIEM_THU_COST_TOTAL.ProjectId = dbo.T_PS_PROJECT.ID AND 
                         dbo.V_GTSL_NGHIEM_THU_COST_TOTAL.TimeId = dbo.V_KY_BAO_CAO.TimeId AND dbo.V_GTSL_NGHIEM_THU_COST_TOTAL.StructureId = dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.StructureId ON 
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR.ProjectId = dbo.T_PS_PROJECT.ID AND dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR.TimeId = dbo.V_KY_BAO_CAO.TimeId AND 
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR.StructureId = dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.StructureId ON dbo.V_GTSL_TH_COST_TOTAL.ProjectId = dbo.T_PS_PROJECT.ID AND 
                         dbo.V_GTSL_TH_COST_TOTAL.TimeId = dbo.V_KY_BAO_CAO.TimeId
GO
/****** Object:  View [dbo].[BI_V_DS_CTDA_DA]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BI_V_DS_CTDA_DA]
AS
SELECT        YEAR(dbo.T_PS_PROJECT.START_DATE) AS NAM, dbo.T_PS_PROJECT.CODE AS MA_DA, dbo.T_PS_PROJECT.NAME AS TEN_DA, dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.HANG_MUC, 
                         dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.MA_HANG_MUC, dbo.T_PS_PROJECT_STRUCT.TOTAL AS NGAN_SACH
FROM            dbo.T_PS_PROJECT INNER JOIN
                         dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2 ON dbo.T_PS_PROJECT.ID = dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.ProjectId INNER JOIN
                         dbo.T_PS_PROJECT_STRUCT ON dbo.V_HANG_MUC_CAY_CAU_TRUC_LV2.StructureId = dbo.T_PS_PROJECT_STRUCT.ID AND dbo.T_PS_PROJECT.ID = dbo.T_PS_PROJECT_STRUCT.PROJECT_ID
GO
/****** Object:  View [dbo].[V_GTSL_KH_BOQ]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_KH_BOQ]
AS
SELECT        dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.ProjectId, SUM(dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.TotalPlanCost * dbo.T_PS_PROJECT_STRUCT.PRICE) AS GTSL_KH_BOQ, 
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.TimeId
FROM            dbo.T_PS_PROJECT_STRUCT INNER JOIN
                         dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ ON dbo.T_PS_PROJECT_STRUCT.ID = dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.StructureId
GROUP BY dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.ProjectId, dbo.V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ.TimeId
GO
/****** Object:  View [dbo].[V_GTSL_NT_BOQ]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_NT_BOQ]
AS
SELECT        SUM(A.VALUE) AS SUM_VALUE, B.PROJECT_ID, B.TIME_PERIOD_ID
FROM            dbo.T_PS_VOLUME_ACCEPT_DETAIL AS A INNER JOIN
                         dbo.T_PS_VOLUME_ACCEPT AS B ON A.HEADER_ID = B.ID INNER JOIN
                         dbo.T_PS_TIME AS C ON B.TIME_PERIOD_ID = C.ID
WHERE        (B.IS_CUSTOMER = '1') AND (B.STATUS = '05')
GROUP BY B.PROJECT_ID, B.TIME_PERIOD_ID
GO
/****** Object:  View [dbo].[V_GTSL_TH_BOQ]    Script Date: 27-Apr-22 11:36:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_GTSL_TH_BOQ]
AS
SELECT        SUM(A.VALUE) AS SUM_VALUE, B.PROJECT_ID, B.TIME_PERIOD_ID
FROM            dbo.T_PS_VOLUME_WORK_DETAIL AS A INNER JOIN
                         dbo.T_PS_VOLUME_WORK AS B ON A.HEADER_ID = B.ID INNER JOIN
                         dbo.T_PS_TIME AS C ON B.TIME_PERIOD_ID = C.ID
WHERE        (B.IS_CUSTOMER = '1') AND (B.STATUS = '05')
GROUP BY B.PROJECT_ID, B.TIME_PERIOD_ID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[34] 4[27] 2[29] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "V_CAP_DA"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 102
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 120
               Left = 255
               Bottom = 250
               Right = 546
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "V_TRANG_THAI_DA"
            Begin Extent = 
               Top = 102
               Left = 38
               Bottom = 198
               Right = 217
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_HOP_DONG_CHINH"
            Begin Extent = 
               Top = 6
               Left = 716
               Bottom = 102
               Right = 886
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_GTSL_TH_BOQ_TOTAL"
            Begin Extent = 
               Top = 6
               Left = 508
               Bottom = 119
               Right = 678
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_GTSL_NGHIEM_THU_BOQ_TOTAL"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 119
               Right = 470
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER"
            Begin Extent = 
               Top = 6
               Left = 1132
               Bo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'ttom = 136
               Right = 1302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_KY_BAO_CAO"
            Begin Extent = 
               Top = 6
               Left = 924
               Bottom = 136
               Right = 1094
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[45] 4[12] 2[30] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "V_GTSL_NGHIEM_THU_COST_TOTAL"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 232
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_CAP_DA"
            Begin Extent = 
               Top = 227
               Left = 1011
               Bottom = 323
               Right = 1181
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_KY_BAO_CAO"
            Begin Extent = 
               Top = 204
               Left = 598
               Bottom = 334
               Right = 768
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 686
               Bottom = 136
               Right = 977
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR"
            Begin Extent = 
               Top = 6
               Left = 1015
               Bottom = 136
               Right = 1185
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_TRANG_THAI_DA"
            Begin Extent = 
               Top = 6
               Left = 1223
               Bottom = 102
               Right = 1402
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_HANG_MUC_CAY_CAU_TRUC_LV2"
            Begin Extent = 
               Top = 141
               Left = 258
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_CP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Bottom = 365
               Right = 509
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_GTSL_TH_COST_TOTAL"
            Begin Extent = 
               Top = 127
               Left = 1224
               Bottom = 257
               Right = 1394
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2370
         Table = 5385
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_CP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_CP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 205
               Right = 326
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 125
               Left = 369
               Bottom = 315
               Right = 565
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_HANG_MUC_CAY_CAU_TRUC_LV2"
            Begin Extent = 
               Top = 6
               Left = 598
               Bottom = 222
               Right = 905
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_HOP_DONG_KHACH_HANG"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "V_DV"
            Begin Extent = 
               Top = 6
               Left = 575
               Bottom = 102
               Right = 745
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_PB"
            Begin Extent = 
               Top = 6
               Left = 783
               Bottom = 102
               Right = 953
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_CAP_DA"
            Begin Extent = 
               Top = 6
               Left = 991
               Bottom = 102
               Right = 1161
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_LOAI_DA"
            Begin Extent = 
               Top = 6
               Left = 1199
               Bottom = 102
               Right = 1369
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_GTHD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_GTHD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BI_V_DS_CTDA_GTHD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_ACCEPT"
            Begin Extent = 
               Top = 8
               Left = 504
               Bottom = 138
               Right = 702
            End
            DisplayFlags = 280
            TopColumn = 7
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2850
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_ACCEPT_VOLUME_PERIOD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_ACCEPT_VOLUME_PERIOD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_MD_PROJECT_LEVEL"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_CAP_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_CAP_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 329
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_AD_ORGANIZE"
            Begin Extent = 
               Top = 6
               Left = 367
               Bottom = 136
               Right = 568
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2055
         Alias = 1710
         Table = 2730
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_DV'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_DV'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 284
               Bottom = 136
               Right = 477
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_BOQ"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_KH_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_KH_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_ACCEPT_DETAIL"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 290
               Bottom = 136
               Right = 499
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_ACCEPT_VOLUME_PERIOD"
            Begin Extent = 
               Top = 6
               Left = 537
               Bottom = 119
               Right = 730
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NGHIEM_THU_BOQ_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NGHIEM_THU_BOQ_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_ACCEPT_DETAIL"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 290
               Bottom = 136
               Right = 499
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_ACCEPT_VOLUME_PERIOD"
            Begin Extent = 
               Top = 6
               Left = 537
               Bottom = 119
               Right = 730
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 2100
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NGHIEM_THU_COST_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NGHIEM_THU_COST_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 290
               Bottom = 136
               Right = 504
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 6
               Left = 542
               Bottom = 136
               Right = 728
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NT_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_NT_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "A"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "B"
            Begin Extent = 
               Top = 6
               Left = 290
               Bottom = 136
               Right = 504
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "C"
            Begin Extent = 
               Top = 6
               Left = 542
               Bottom = 136
               Right = 728
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_BOQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_WORK_DETAIL"
            Begin Extent = 
               Top = 7
               Left = 346
               Bottom = 215
               Right = 701
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 244
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 17
         End
         Begin Table = "V_WORK_VOLUME_PERIOD"
            Begin Extent = 
               Top = 15
               Left = 739
               Bottom = 191
               Right = 1054
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 4155
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_BOQ_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_BOQ_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_WORK_DETAIL"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 290
               Bottom = 136
               Right = 499
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V_WORK_VOLUME_PERIOD"
            Begin Extent = 
               Top = 6
               Left = 537
               Bottom = 119
               Right = 723
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 2445
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_COST_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_GTSL_TH_COST_TOTAL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ps"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HANG_MUC_CAY_CAU_TRUC_LV2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HANG_MUC_CAY_CAU_TRUC_LV2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_CONTRACT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2910
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HOP_DONG_CHINH'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HOP_DONG_CHINH'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "child"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "parent"
            Begin Extent = 
               Top = 6
               Left = 305
               Bottom = 136
               Right = 534
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2910
         Alias = 1410
         Table = 1830
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HOP_DONG_KHACH_HANG'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_HOP_DONG_KHACH_HANG'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 329
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_TIME"
            Begin Extent = 
               Top = 6
               Left = 367
               Bottom = 136
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 4
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_KY_BAO_CAO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_KY_BAO_CAO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_MD_PROJECT_TYPE"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 537
            End
            DisplayFlags = 280
            TopColumn = 11
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 3585
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_LOAI_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_LOAI_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 329
            End
            DisplayFlags = 280
            TopColumn = 20
         End
         Begin Table = "T_AD_ORGANIZE"
            Begin Extent = 
               Top = 6
               Left = 367
               Bottom = 136
               Right = 568
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PLAN_COST"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 262
            End
            DisplayFlags = 280
            TopColumn = 6
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 300
               Bottom = 136
               Right = 509
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 4260
         Alias = 2505
         Table = 2835
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_CUSTOMER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PLAN_COST"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 262
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "T_PS_PROJECT_STRUCT"
            Begin Extent = 
               Top = 6
               Left = 300
               Bottom = 136
               Right = 509
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 2355
         Alias = 2505
         Table = 2835
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_PROJECT_STRUCTURE_TOTAL_PLAN_COST_VENDOR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_PROJECT"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 329
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_TRANG_THAI_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_TRANG_THAI_DA'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T_PS_VOLUME_WORK"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 2880
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_WORK_VOLUME_PERIOD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_WORK_VOLUME_PERIOD'
GO
