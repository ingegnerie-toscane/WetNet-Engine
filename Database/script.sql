SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE SCHEMA IF NOT EXISTS `wetdb` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci ;
USE `wetdb` ;

-- -----------------------------------------------------
-- Table `wetdb`.`connections`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`connections` (
  `id_connections` INT NOT NULL,
  `odbc_dsn` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  PRIMARY KEY (`id_connections`),
  UNIQUE INDEX `id_connections_UNIQUE` (`id_connections` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`measures`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`measures` (
  `name` VARCHAR(255) NOT NULL,
  `type` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  `table` VARCHAR(255) NULL,
  `table_column` VARCHAR(255) NULL,
  `connections_id_connections` INT NOT NULL,
  PRIMARY KEY (`connections_id_connections`, `name`),
  INDEX `fk_measures_connections1_idx` (`connections_id_connections` ASC),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC),
  CONSTRAINT `fk_measures_connections1`
    FOREIGN KEY (`connections_id_connections`)
    REFERENCES `wetdb`.`connections` (`id_connections`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`districts`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`districts` (
  `name` VARCHAR(255) NOT NULL,
  `municipality` VARCHAR(255) NULL,
  PRIMARY KEY (`name`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`data_measures`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`data_measures` (
  `timestamp` DATETIME NOT NULL,
  `value` DOUBLE NULL,
  `measures_connections_id_connections` INT NOT NULL,
  `measures_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`measures_connections_id_connections`, `measures_name`, `timestamp`),
  INDEX `fk_data_measures_measures1_idx` (`measures_connections_id_connections` ASC, `measures_name` ASC),
  CONSTRAINT `fk_data_measures_measures1`
    FOREIGN KEY (`measures_connections_id_connections` , `measures_name`)
    REFERENCES `wetdb`.`measures` (`connections_id_connections` , `name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`data_districts`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`data_districts` (
  `timestamp` DATETIME NOT NULL,
  `value` DOUBLE NULL,
  `districts_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`timestamp`, `districts_name`),
  INDEX `fk_data_districts_districts1_idx` (`districts_name` ASC),
  CONSTRAINT `fk_data_districts_districts1`
    FOREIGN KEY (`districts_name`)
    REFERENCES `wetdb`.`districts` (`name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`measures_has_districts`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`measures_has_districts` (
  `measures_connections_id_connections` INT NOT NULL,
  `measures_name` VARCHAR(255) NOT NULL,
  `districts_name` VARCHAR(255) NOT NULL,
  `sign` VARCHAR(255) NULL,
  PRIMARY KEY (`measures_connections_id_connections`, `measures_name`, `districts_name`),
  INDEX `fk_measures_has_districts_districts1_idx` (`districts_name` ASC),
  INDEX `fk_measures_has_districts_measures1_idx` (`measures_connections_id_connections` ASC, `measures_name` ASC),
  CONSTRAINT `fk_measures_has_districts_measures1`
    FOREIGN KEY (`measures_connections_id_connections` , `measures_name`)
    REFERENCES `wetdb`.`measures` (`connections_id_connections` , `name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_measures_has_districts_districts1`
    FOREIGN KEY (`districts_name`)
    REFERENCES `wetdb`.`districts` (`name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`day_statistic`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`day_statistic` (
  `day` DATE NOT NULL,
  `min_night` DOUBLE NULL,
  `min_day` DOUBLE NULL,
  `max_day` DOUBLE NULL,
  `avg_day` DOUBLE NULL,
  `districts_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`day`, `districts_name`),
  UNIQUE INDEX `day_UNIQUE` (`day` ASC),
  INDEX `fk_day_statistic_districts1_idx` (`districts_name` ASC),
  CONSTRAINT `fk_day_statistic_districts1`
    FOREIGN KEY (`districts_name`)
    REFERENCES `wetdb`.`districts` (`name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`users` (
  `idusers` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NULL,
  `surname` VARCHAR(255) NULL,
  `email` VARCHAR(255) NULL,
  `districts_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`idusers`, `districts_name`),
  UNIQUE INDEX `idusers_UNIQUE` (`idusers` ASC),
  INDEX `fk_users_districts1_idx` (`districts_name` ASC),
  CONSTRAINT `fk_users_districts1`
    FOREIGN KEY (`districts_name`)
    REFERENCES `wetdb`.`districts` (`name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`alarm_threshold`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`alarm_threshold` (
  `idalarm_threshold` INT NOT NULL AUTO_INCREMENT,
  `enabled` BIT NULL,
  `min` DOUBLE NULL,
  `max` DOUBLE NULL,
  `districts_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`idalarm_threshold`, `districts_name`),
  INDEX `fk_alarm_threshold_districts1_idx` (`districts_name` ASC),
  CONSTRAINT `fk_alarm_threshold_districts1`
    FOREIGN KEY (`districts_name`)
    REFERENCES `wetdb`.`districts` (`name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`lcf_identities`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`lcf_identities` (
  `table_name` VARCHAR(255) NOT NULL,
  `description` VARCHAR(255) NULL,
  `municipality` VARCHAR(255) NULL,
  PRIMARY KEY (`table_name`),
  UNIQUE INDEX `table_name_UNIQUE` (`table_name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `wetdb`.`lcf_data`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `wetdb`.`lcf_data` (
  `timestamp` DATETIME NOT NULL,
  `Q` DOUBLE NULL,
  `P` DOUBLE NULL,
  `lcf_identities_table_name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`timestamp`, `lcf_identities_table_name`),
  INDEX `fk_lcf_data_lcf_identities1_idx` (`lcf_identities_table_name` ASC),
  CONSTRAINT `fk_lcf_data_lcf_identities1`
    FOREIGN KEY (`lcf_identities_table_name`)
    REFERENCES `wetdb`.`lcf_identities` (`table_name`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE USER 'wet_admin' IDENTIFIED BY 'wet_admin';

GRANT SELECT, INSERT, TRIGGER, UPDATE, DELETE ON TABLE `wetdb`.* TO 'wet_admin';
CREATE USER 'wet_user' IDENTIFIED BY 'wet_user';

GRANT SELECT ON TABLE `wetdb`.* TO 'wet_user';

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
