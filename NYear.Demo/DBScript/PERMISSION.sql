create table SEQUENCE_TABLE (
   SEQUENCE_NAME        varchar(32)                    null,
   CURRENCE_VALUE       numeric(16,0)                  null,
   SETVAL               numeric(16,0)                  null
)


INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ('PRM_USER_ROLE_SEQ',1,1);
INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ('PRM_ROLE_AUTHORIZE_SEQ',1,1);
INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ('PRM_PERMISSION_SEQ',1,1);
INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ('PRM_ROLE_SEQ',1,1);
INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ('PRM_USER_AUTHORIZE_SEQ',1,1);
INSERT INTO SEQUENCE_TABLE (SEQUENCE_NAME,CURRENCE_VALUE,SETVAL) VALUES ( 'FC_FLOW_CHART_SEQ',1,1)


drop table SEQUENCE_TABLE
drop table PRM_PERMISSION
drop table PRM_ROLE
drop table PRM_ROLE_AUTHORIZE
drop table PRM_USER_AUTHORIZE
drop table PRM_USER_ROLE
DROP VIEW V_PRM_USER_AUTHORIZE

/*==============================================================*/
/* Table: PRM_PERMISSION                                        */
/*==============================================================*/
create table PRM_PERMISSION (
   RESOURCE_NAME        char(64)             not null,
   OPERATE_NAME         char(64)             not null,
   DESCRIPT             varchar(200)         null,
   CREATE_DATE          datetime             null,
   CREATE_BY            varchar(100)         null,
   constraint PK_PRM_PERMISSION primary key (RESOURCE_NAME, OPERATE_NAME)
)


/*==============================================================*/
/* Table: PRM_ROLE                                              */
/*==============================================================*/
create table PRM_ROLE (
   ROLE_NAME            char(64)             not null,
   IS_SUPPER_ADMIN      char(1)              default 'N',
   DESCRIPT             varchar(200)         null,
   CREATE_DATE          datetime             null,
   CREATE_BY            varchar(100)         null,
   constraint PK_PRM_ROLE primary key (ROLE_NAME)
)


/*==============================================================*/
/* Table: PRM_ROLE_AUTHORIZE                                    */
/*==============================================================*/
create table PRM_ROLE_AUTHORIZE (
   ROLE_NAME            char(64)             not null,
   RESOURCE_NAME        char(64)             not null,
   OPERATE_NAME         char(64)             not null,
   IS_FORBIDDEN         char(1)              default 'N',
   CREATE_DATE          datetime             null,
   CREATE_BY            varchar(100)         null,
   constraint PK_PRM_ROLE_AUTHORIZE primary key (ROLE_NAME, RESOURCE_NAME, OPERATE_NAME)
)


/*==============================================================*/
/* Table: PRM_USER_AUTHORIZE                                    */
/*==============================================================*/
create table PRM_USER_AUTHORIZE (
   USER_ID              char(64)             not null,
   USER_NAME            varchar(64)          null,
   RESOURCE_NAME        char(64)             not null,
   OPERATE_NAME         char(64)             not null,
   IS_FORBIDDEN         char(1)              default 'N',
   CREATE_DATE          datetime             null,
   CREATE_BY            varchar(100)         null,
   constraint PK_PRM_USER_AUTHORIZE primary key (USER_ID, RESOURCE_NAME, OPERATE_NAME)
)

/*==============================================================*/
/* Table: PRM_USER_ROLE                                         */
/*==============================================================*/
create table PRM_USER_ROLE (
   USER_ID              char(64)             not null,
   USER_NAME            varchar(64)          null,
   ROLE_NAME            char(64)             not null,
   CREATE_DATE          datetime             null,
   CREATE_BY            varchar(100)         null,
   constraint PK_PRM_USER_ROLE primary key (USER_ID, ROLE_NAME)
)

/*==============================================================*/
/* View: V_PRM_USER_AUTHORIZE                               */
/*==============================================================*/
create view V_PRM_USER_AUTHORIZE as
SELECT UR.USER_ID,UR.USER_NAME, RA.RESOURCE_NAME,RA.OPERATE_NAME,RA.IS_FORBIDDEN,P.DESCRIPT
FROM PRM_ROLE_AUTHORIZE RA ,PRM_USER_ROLE UR,PRM_PERMISSION P
WHERE RA.ROLE_NAME = UR.ROLE_NAME
AND RA.RESOURCE_NAME = P.RESOURCE_NAME
AND RA.OPERATE_NAME = P.OPERATE_NAME
UNION
SELECT UA.USER_ID,UA.USER_NAME,UA.RESOURCE_NAME,UA.OPERATE_NAME ,UA.IS_FORBIDDEN,P.DESCRIPT
FROM PRM_USER_AUTHORIZE UA ,PRM_PERMISSION P
WHERE UA.RESOURCE_NAME = P.RESOURCE_NAME
AND UA.OPERATE_NAME = P.OPERATE_NAME