DROP DATABASE ProductService;
DROP DATABASE OrderService;

CREATE DATABASE ProductService;
CREATE DATABASE OrderService;

GO

USE ProductService;

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


GO



