

SELECT * FROM MenuItem


SET IDENTITY_INSERT [MenuItem] ON
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (1,N'Admin',N'#',null,1,'fas fa-user-circle')
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (2,N'Master',N'#',null ,1,'fa fa-info-circle')
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (100,N'SuperAdmin',N'#',null,1,'fas fa-user-tie')
SET IDENTITY_INSERT [MenuItem] OFF

SET IDENTITY_INSERT [MenuItem] ON
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (101,N'User Rights',N'/AdminRights/Index',1,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (102,N'Manage Users',N'/Auth/UserList',1,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (103,N'Maintain Roles',N'/Auth/RoleList',1,1,null)
SET IDENTITY_INSERT [MenuItem] OFF

SET IDENTITY_INSERT [MenuItem] ON
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (201,N'Data Types Config',N'/DataTypes/Index',2,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (202,N'Form Config',N'/FormDetails/Index',2,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (203,N'Messages',N'/FormValues/Index',2,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (204,N'Business Info',N'/BusinessInfo/Index',2,1,null)
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (250,N'Data Entry Check',N'/FormValues/DynamicForm',2,1,null)
SET IDENTITY_INSERT [MenuItem] OFF

