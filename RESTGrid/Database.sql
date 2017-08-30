CREATE DATABASE  IF NOT EXISTS `restgrid` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `restgrid`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: restgrid
-- ------------------------------------------------------
-- Server version	5.7.17-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `history`
--

DROP TABLE IF EXISTS `history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `history` (
  `Body` json DEFAULT NULL,
  `CustomProperties` json DEFAULT NULL,
  `Event` json NOT NULL,
  `Timestamp` datetime NOT NULL,
  `idWorkflowType` int(11) DEFAULT NULL,
  `idWorkflow` char(36) DEFAULT NULL,
  `SplitID` varchar(100) DEFAULT NULL,
  KEY `FK_History_WorkflowType` (`idWorkflowType`),
  KEY `Idx_History_WorkflowID` (`idWorkflow`),
  KEY `Idx_History_WF_Split` (`idWorkflow`,`SplitID`),
  CONSTRAINT `FK_History_WorkflowType` FOREIGN KEY (`idWorkflowType`) REFERENCES `workflowtype` (`idWorkflowType`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `queue`
--

DROP TABLE IF EXISTS `queue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `queue` (
  `idWorkflow` char(36) NOT NULL,
  `idQueue` char(36) NOT NULL,
  `idWorkflowType` int(11) NOT NULL,
  `CustomProperties` json DEFAULT NULL,
  `Timestamp` datetime NOT NULL,
  `StepIdentifier` varchar(500) DEFAULT NULL,
  `Success` int(1) NOT NULL,
  `Body` json DEFAULT NULL,
  `Active` int(1) DEFAULT NULL,
  `Retries` int(11) DEFAULT NULL,
  `SplitID` varchar(100) DEFAULT '0',
  PRIMARY KEY (`idQueue`),
  KEY `FK_Workflow_WorkflowType_idx` (`idWorkflowType`),
  KEY `Idx_WorkflowID` (`idWorkflow`),
  KEY `Idx_WorkflowID_SplitID` (`idWorkflow`,`SplitID`),
  CONSTRAINT `FK_Workflow_WorkflowType` FOREIGN KEY (`idWorkflowType`) REFERENCES `workflowtype` (`idWorkflowType`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transformer`
--

DROP TABLE IF EXISTS `transformer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transformer` (
  `idTransformer` int(11) NOT NULL AUTO_INCREMENT,
  `TransformerJson` json NOT NULL,
  `Timestamp` datetime NOT NULL,
  PRIMARY KEY (`idTransformer`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `workflowtype`
--

DROP TABLE IF EXISTS `workflowtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `workflowtype` (
  `idWorkflowType` int(11) NOT NULL AUTO_INCREMENT,
  `BusinessLogic` json NOT NULL,
  `Timestamp` datetime NOT NULL,
  `Name` varchar(500) NOT NULL,
  PRIMARY KEY (`idWorkflowType`),
  UNIQUE KEY `Name_UNIQUE` (`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'restgrid'
--

--
-- Dumping routines for database 'restgrid'
--
/*!50003 DROP PROCEDURE IF EXISTS `Administration_CreateTransformer` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Administration_CreateTransformer`(IN TransformerJson JSON)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    START TRANSACTION;
    
        
    INSERT INTO `transformer` (`Timestamp`,`TransformerJson`)
	VALUES (UTC_TIMESTAMP(),TransformerJson);   
    
	COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Administration_CreateWorkflowType` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Administration_CreateWorkflowType`(IN WorkflowTypeName VARCHAR(500),IN BusinessLogic JSON)
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION 
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    START TRANSACTION;
    
        
    INSERT INTO `workflowtype` (`Name`,`Timestamp`,`BusinessLogic`)
	VALUES (WorkflowTypeName,UTC_TIMESTAMP(),BusinessLogic);   
    
	COMMIT;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Administration_GetHistory` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Administration_GetHistory`(IN WorkflowID CHAR(36))
BEGIN

DECLARE EXIT HANDLER FOR SQLEXCEPTION 
	BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

	START TRANSACTION;   

	SELECT H.`Body`,
    H.`CustomProperties`,
    H.`Event`,
    H.`Timestamp`,
    H.`idWorkflowType`,
    H.`idWorkflow`,
    H.`SplitID`,
	W.`idWorkflowType`,
    W.`BusinessLogic`,
    W.`Name`FROM `history` H, `workflowtype` W
    WHERE `idWorkflow` = WorkflowID 
    AND H.`idWorkflowType` = W.`idWorkflowType`; 

   
	COMMIT;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Orchestration_Enqueue` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Orchestration_Enqueue`()
BEGIN
DECLARE EXIT HANDLER FOR SQLEXCEPTION 

    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

	START TRANSACTION;
    
    DROP TABLE IF EXISTS Temp;
    
    CREATE TEMPORARY TABLE Temp AS (
	SELECT Q.`idWorkflow`,
    Q.`idQueue` As QueueID,
    Q.`idWorkflowType`,
    Q.`CustomProperties`,
    Q.`Timestamp`,
    Q.`StepIdentifier`,
    Q.`Success`,
    Q.`Body`,
    Q.`Retries`,
    WT.`BusinessLogic`,
    WT.`Name` As WorkflowTypeName,
    Q.`SplitID`
	FROM `queue` Q
    INNER JOIN `workflowtype` WT ON WT.`idWorkflowType` = Q.`idWorkflowType`   
    WHERE  Q.`Active` = 1);   
    
    SET SQL_SAFE_UPDATES=0;
    DELETE FROM `queue` WHERE `idQueue` IN (SELECT DISTINCT QueueID FROM Temp);
    SET SQL_SAFE_UPDATES=1;
    
    SELECT * FROM Temp ORDER BY `idWorkflow`;

    COMMIT;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Orchestration_PublishWorkflowStep` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Orchestration_PublishWorkflowStep`(IN WorkflowTypeName VARCHAR(500),IN WorkflowID CHAR(36),IN MessageBody JSON,IN CustomProperties JSON, IN StepIdentifier VARCHAR(500), IN Success INT(1), IN WorkflowCompleted INT(1), 
	IN Retries INT, IN Active INT(1),IN RunStepIdentifier VARCHAR(500), IN In_SplitID VARCHAR(100))
BEGIN


DECLARE v_WorkflowTypeID INT;
DECLARE v_Event JSON;
DECLARE v_SequenceNumber INT;

DECLARE EXIT HANDLER FOR SQLEXCEPTION 
	BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

	START TRANSACTION;
   

	SELECT idWorkflowType INTO v_WorkflowTypeID FROM `workflowtype`
    WHERE `Name` = WorkflowTypeName;    

	IF(v_WorkflowTypeID IS NULL)
    THEN
		SIGNAL SQLSTATE '45000'
		SET MESSAGE_TEXT = 'Invalid Workflow Type';
    END IF;    

    

    IF(WorkflowCompleted = 0)
    THEN
    BEGIN
    
		SELECT COUNT(*) INTO v_SequenceNumber FROM `history` 
		WHERE `idWorkflow` = WorkflowID 
		AND `SplitID` = In_SplitID; 
        
		INSERT INTO `queue`(`idWorkflow`,`idQueue`,`idWorkflowType`,`CustomProperties`,`Timestamp`,`StepIdentifier`,`Success`,`Body`,`Retries`,`Active`,`SplitID`)
		VALUES(WorkflowID,UUID(),v_WorkflowTypeID,CustomProperties,UTC_TIMESTAMP(),StepIdentifier,Success,MessageBody,Retries,Active,In_SplitID);

		IF(RunStepIdentifier IS NULL)
        THEN        
			SET v_Event = CONCAT('{"WorkflowState":"Started","SequenceNumber":',v_SequenceNumber+1,'}');
        ELSE 
			SET v_Event = CONCAT('{"StepID":"',RunStepIdentifier,'","Success":"',Success,'","WorkflowState":"InProcess","SequenceNumber":',v_SequenceNumber+1,'}');
        END IF;
        
	END;

    ELSE
		SET v_Event = CONCAT('{"WorkflowState":"Completed"}');
	END IF; 

    
    INSERT INTO `history`(`idWorkflow`,`Body`,`CustomProperties`,`Event`,`Timestamp`,`idWorkflowType`,`SplitID`)
	VALUES (WorkflowID,MessageBody,CustomProperties,v_Event,UTC_Timestamp,v_WorkflowTypeID,In_SplitID);
    
	COMMIT;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `Orchestration_SetActive` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `Orchestration_SetActive`(IN MessageBody JSON,IN In_CustomPropertyName VARCHAR(500),IN In_CustomPropertyValue VARCHAR(500))
BEGIN

DECLARE done INT DEFAULT FALSE;
DECLARE v_Event JSON;
DECLARE v_SequenceNumber INT;
DECLARE v_IdWorkflowType INT;
DECLARE t_idWorkflow CHAR(36);
DECLARE t_splitId VARCHAR(100);

DECLARE cur CURSOR FOR SELECT `idWorkflow`,`SplitID` FROM `queue`
WHERE json_extract(`CustomProperties`, concat('$.',In_CustomPropertyName)) = In_CustomPropertyValue;

DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

DECLARE EXIT HANDLER FOR SQLEXCEPTION 

    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

	START TRANSACTION;	     

    OPEN cur;

    read_loop: LOOP

    FETCH cur INTO t_idWorkflow,t_splitId;    

    IF done THEN
      LEAVE read_loop;
    END IF;
    

	IF(MessageBody IS NOT NULL)

    THEN
		UPDATE queue SET Active = 1,Retries = 0, Body = MessageBody, Success = 1
		WHERE idWorkflow = t_idWorkflow;
    ELSE
		UPDATE queue SET Active = 1,Retries = 0, Success = 1
		WHERE idWorkflow = t_idWorkflow;
    END IF;    

    SELECT COUNT(*) INTO v_SequenceNumber FROM `history`
    WHERE `idWorkflow` = t_idWorkflow AND `SplitID` = t_splitId;

    SELECT idWorkflowType INTO v_IdWorkflowType FROM queue WHERE idWorkflow = t_idWorkflow LIMIT 1;   

	SET v_Event = CONCAT('{"CustomPropertyName":"',In_CustomPropertyName ,'","CustomPropertyValue":"',In_CustomPropertyValue,'","WorkflowState":"ReStarted","SequenceNumber":',v_SequenceNumber+1,'}');

    INSERT INTO `history`(`idWorkflow`,`Body`,`Event`,`Timestamp`,`idWorkflowType`,`SplitID`)
	VALUES(t_idWorkflow,MessageBody,v_Event,UTC_Timestamp,v_IdWorkflowType,t_splitId);

	END LOOP;

	CLOSE cur;    

    

	COMMIT;

END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-08-30 20:41:17
