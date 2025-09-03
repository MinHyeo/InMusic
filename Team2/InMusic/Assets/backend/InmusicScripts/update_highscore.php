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

$userId = $_POST['userId'];
$musicId = $_POST['musicId'];
$newScore = intval($_POST['musicScore']);
$newCombo = intval($_POST['musicCombo']);
$newAccuracy = floatval($_POST['musicAccuracy']);
$newRate = $_POST['musicRate'];

$sql = "UPDATE musiclog 
        SET musicScore = '$newScore', musicCombo = $newCombo, musicAccuracy = $newAccuracy, musicRank = '$newRate'
        WHERE userID = '$userId' AND musicID = '$musicId'";

if ($conn->query($sql) === TRUE) {
    echo "success";
} else {
    echo "error";
}

$conn->close();
?>
