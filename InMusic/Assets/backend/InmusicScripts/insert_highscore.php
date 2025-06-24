<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "inmusic";  // phpMyAdmin에서 확인한 DB명

// MySQL 연결
$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// POST 데이터 받기 (안전하게)
$userId = isset($_POST['userId']) ? $_POST['userId'] : '';
$musicId = isset($_POST['musicId']) ?$_POST['musicId']: '';
$newScore = isset($_POST['musicScore']) ? intval($_POST['musicScore']) : 0;
$newCombo = isset($_POST['musicCombo']) ? intval($_POST['musicCombo']) : 0;
$newAccuracy = isset($_POST['musicAccuracy']) ? floatval($_POST['musicAccuracy']) : 0.0;
$newRate = isset($_POST['musicRate']) ? $_POST['musicRate'] : '';


// 기존 기록 있는지 확인


    $insert_sql = "INSERT INTO musiclog (userID, musicID, musicScore, musicCombo, musicAccuracy, musicRank) 
                   VALUES ('$userId', '$musicId', $newScore, $newCombo, $newAccuracy, '$newRate')";

    if ($conn->query($insert_sql) === TRUE) {
        echo "insert_success";
    } else {
        echo "error: " . $conn->error;
    }


$conn->close();
?>