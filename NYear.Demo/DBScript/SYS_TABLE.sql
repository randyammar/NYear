drop table  SYS_USER;
drop table  SYS_RESOURCE;
drop table  SYS_ROLE;
drop table SYS_DATA_DICTIONARY;
drop table  SYS_USER_AUTHORIZATION;
drop table  SYS_ROLE_AUTHORIZATION;
drop table SYS_USER_ROLE;

CREATE TABLE SYS_USER
(
STATUS CHAR(1) DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
USER_ACCOUNT CHAR(36) NOT NULL,
USER_NAME VARCHAR(80) NOT NULL,
USER_PASSWORD VARCHAR(200),
EMAIL_ADDR VARCHAR(200),
PHONE_NO VARCHAR(80),
ADDRESS VARCHAR(200),
FE_MALE CHAR(1) NULL,
FAIL_TIMES INT  DEFAULT 0 NOT NULL,
IS_LOCKED CHAR(1) DEFAULT 'N' NOT NULL ,
PRIMARY KEY (USER_ACCOUNT)
);

CREATE TABLE SYS_RESOURCE
(
STATUS CHAR(1)  DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
ID  CHAR(36)  NOT NULL,
RESOURCE_NAME VARCHAR(80),
RESOURCE_DESC VARCHAR(2000),
RESOURCE_TYPE CHAR(36)  DEFAULT 'WEB' NOT NULL,---/WEB/WFP_PAGE/WPF_WIN/WIN_FORM/CLASS/SERVICE/CONF/OPERATE/DATA/MENU
RESOURCE_SCOPE CHAR(36) DEFAULT 'GENERAL' NOT NULL ,---/GENERAL/PC/APP/PDA/KANBAN/REPORT
RESOURCE_LOCATION  VARCHAR(600) NOT NULL ,
PARENT_ID   VARCHAR(36),
PARENT_ID1  VARCHAR(36),
PARENT_ID2  VARCHAR(36),
PARENT_ID3  VARCHAR(36),
PARENT_ID4  VARCHAR(36),
PARENT_ID5  VARCHAR(36),
PARENT_ID6  VARCHAR(36),
RESOURCE_INDEX INT,
PRIMARY KEY (ID), 
CONSTRAINT SYS_RESOURCE$UNIQUE UNIQUE (RESOURCE_TYPE,RESOURCE_SCOPE,RESOURCE_LOCATION)
);

 
CREATE TABLE SYS_ROLE
 (
STATUS CHAR(1)  DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
ROLE_CODE CHAR(36) NOT NULL,
ROLE_NAME VARCHAR(80),
ROLE_DESC VARCHAR(2000),
PRIMARY KEY (ROLE_CODE)
 );

 
CREATE TABLE SYS_USER_AUTHORIZATION
(
STATUS CHAR(1)  DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
USER_ACCOUNT  CHAR(36)  NOT NULL,
RESOURCE_ID CHAR(36)  NOT NULL,
IS_FORBIDDEN CHAR(1)  DEFAULT 'N' NOT NULL,
PRIMARY KEY (USER_ACCOUNT,RESOURCE_ID)
);


CREATE TABLE SYS_ROLE_AUTHORIZATION
(
STATUS CHAR(1)  DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
ROLE_CODE CHAR(36)  NOT NULL,
RESOURCE_ID CHAR(36)  NOT NULL,
IS_FORBIDDEN CHAR(1) DEFAULT 'N' NOT NULL ,
PRIMARY KEY (ROLE_CODE,RESOURCE_ID)
);


CREATE TABLE SYS_USER_ROLE
(
STATUS CHAR(1)  DEFAULT 'O' NOT NULL,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
USER_ACCOUNT  CHAR(36)  NOT NULL,
ROLE_CODE CHAR(36)  NOT NULL,
PRIMARY KEY (USER_ACCOUNT,ROLE_CODE)
);


CREATE TABLE SYS_DATA_DICTIONARY
(
STATUS CHAR(1) DEFAULT 'O' NOT NULL ,
CREATED_BY  VARCHAR(80),
CREATED_DATE DATETIME,
LAST_UPDATED_BY VARCHAR(80),
LAST_UPDATED_DATE DATETIME,
TABLE_CODE  CHAR(36)  NOT NULL,
COLUMN_CODE CHAR(36)  DEFAULT '-' NOT NULL,
OBJ_NAME  VARCHAR(80),
OBJ_DESC  VARCHAR(2000) NULL,
ZHCN_NAME VARCHAR(80),
ENUS_NAME VARCHAR(80),
PRIMARY KEY (TABLE_CODE,COLUMN_CODE)
);

CREATE VIEW V_SYS_USER_RESOURCE AS
SELECT T0.USER_ACCOUNT, T0.USER_NAME, T2.IS_FORBIDDEN, T2.RESOURCE_ID,T3.RESOURCE_NAME, T3.RESOURCE_TYPE, T3.RESOURCE_SCOPE, T3.RESOURCE_LOCATION
  FROM SYS_USER T0
  INNER JOIN SYS_USER_ROLE T1 ON T0.USER_ACCOUNT = T1.USER_ACCOUNT AND T1.STATUS = 'O'
  INNER JOIN SYS_ROLE_AUTHORIZATION T2 ON T2.ROLE_CODE = T1.ROLE_CODE AND T2.STATUS = 'O'
  INNER JOIN SYS_RESOURCE T3 ON T3.ID = T2.RESOURCE_ID AND T3.STATUS = 'O'
UNION
SELECT T4.USER_ACCOUNT, T4.USER_NAME, T5.IS_FORBIDDEN, T5.RESOURCE_ID, T6.RESOURCE_NAME, T6.RESOURCE_TYPE, T6.RESOURCE_SCOPE, T6.RESOURCE_LOCATION
  FROM SYS_USER T4
  INNER JOIN SYS_USER_AUTHORIZATION T5 ON T4.USER_ACCOUNT = T5.USER_ACCOUNT AND T5.STATUS = 'O'
  INNER JOIN SYS_RESOURCE T6 ON T6.ID = T5.RESOURCE_ID AND T6.STATUS = 'O';
 

 SQL 
 CREATE FUNCTION GET_RESOURCE_PATH
( 
	@RESOURCE_ID VARCHAR(200)
)
RETURNS  VARCHAR(2000)
AS
BEGIN 
	DECLARE @NAME_PATH  VARCHAR(MAX);
	SELECT @NAME_PATH =RS.RESOURCE_TYPE + RS.RESOURCE_SCOPE + RS.RESOURCE_LOCATION from SYS_RESOURCE rs WHERE RS.ID = @RESOURCE_ID;
	RETURN @NAME_PATH; 
END
GO