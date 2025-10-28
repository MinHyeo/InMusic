-- phpMyAdmin SQL Dump
-- version 5.2.1
-- Host: 127.0.0.1
-- 생성 시간: 25-10-27 10:05
-- 서버 버전: 10.4.32-MariaDB
-- PHP 버전: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

-- 데이터베이스: `inmusic`

-- --------------------------------------------------------

-- 테이블 구조 `music`
CREATE TABLE `music` (
  `musicID` varchar(40) NOT NULL,
  `musicName` varchar(40) NOT NULL,
  `musicArtist` varchar(40) NOT NULL,
PRIMARY KEY (`musicID`)
);

-- --------------------------------------------------------

-- 테이블 구조 `musiclog`
CREATE TABLE `musiclog` (
  `logID` int(11) NOT NULL AUTO_INCREMENT,
  `userID` varchar(20) NOT NULL,
  `musicID` varchar(40) NOT NULL,
  `musicScore` int(11) NOT NULL,
  `musicCombo` int(11) NOT NULL,
  `musicAccuracy` float NOT NULL,
  `musicRank` varchar(10) NOT NULL,
 PRIMARY KEY (`logID`)
);

-- --------------------------------------------------------

-- 테이블 구조 `user`
CREATE TABLE `user` (
  `userID` varchar(20) NOT NULL,
  `userName` varchar(15) NOT NULL,
 PRIMARY KEY (`userID`)
);

-- --------------------------------------------------------