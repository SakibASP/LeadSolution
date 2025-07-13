

SELECT * FROM MenuItem


SET IDENTITY_INSERT [MenuItem] ON
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (1,N'Admin',N'#',null,1,'fas fa-user-circle')
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (2,N'Master',N'#',null ,1,'fa fa-info-circle')
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (100,N'SuperAdmin',N'#',null,1,'fas fa-user-tie')
SET IDENTITY_INSERT [MenuItem] OFF

SET IDENTITY_INSERT [MenuItem] ON
INSERT INTO MenuItem([MenuId],[MenuName],[MenuUrl],[MenuParentId],[Active],[FaIcon]) VALUES (103,N'Maintain Roles',N'/Auth/RoleList',1,1,null)
SET IDENTITY_INSERT [MenuItem] OFF