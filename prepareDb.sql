DROP DATABASE ProductService;
DROP DATABASE OrderService;
DROP DATABASE [aspnet-IdentityService];

CREATE DATABASE ProductService;
CREATE DATABASE OrderService;
CREATE DATABASE [aspnet-IdentityService];
GO

USE ProductService;

DROP USER  IF EXISTS  productManager;
DROP LOGIN productManager;

CREATE LOGIN productManager
    WITH PASSWORD = 'totallynotamangerpassword!@#$5';
CREATE USER productManager FOR LOGIN productManager;

GO

CREATE FUNCTION [GetProductsFromCategoryHierarchy] (@categoryId INT)
RETURNS TABLE
AS
RETURN (
WITH CategoryHierarchy AS ( SELECT c.Id, c.CategoryName, c.ParentCategoryId
        FROM ProductCategories c WHERE c.Id = @categoryId
        UNION ALL
        SELECT pc.Id, pc.CategoryName, pc.ParentCategoryId FROM ProductCategories pc
        INNER JOIN CategoryHierarchy ch ON pc.ParentCategoryId = ch.Id )
        SELECT p.Id, p.CategoryId, p.ConcurrencyStamp, p.Description, p.Name, p.Price, p.Quantity
        FROM Products p
        INNER JOIN  CategoryHierarchy ch ON p.CategoryId = ch.Id;
);

GO

DROP USER  IF EXISTS  productManager;
DROP LOGIN productManager;

CREATE LOGIN productManager
    WITH PASSWORD = 'totallynotamangerpassword!@#$5';
CREATE USER productManager FOR LOGIN productManager;

EXEC sp_addrolemember db_owner, productManager;

USE OrderService;

DROP USER  IF EXISTS  orderManager;
DROP LOGIN orderManager;

CREATE LOGIN orderManager
    WITH PASSWORD = 'totallynotamangerpassword!@#$5';
CREATE USER orderManager FOR LOGIN orderManager;

EXEC sp_addrolemember db_owner, orderManager;

USE [aspnet-IdentityService];

DROP USER  IF EXISTS  identityManager;
DROP LOGIN identityManager;

CREATE LOGIN identityManager
    WITH PASSWORD = 'totallynotamangerpassword!@#$5';
CREATE USER identityManager FOR LOGIN identityManager;

EXEC sp_addrolemember db_owner, identityManager;

ALTER LOGIN sa WITH
  PASSWORD = 'debbugingOnly!@#$5'

GO



