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

$userId = isset($_POST['userId']) ? $_POST['userId'] : '없음';
$musicId = isset($_POST['musicId']) ? $_POST['musicId'] : 0; 
$newScore = isset($_POST['musicScore']) ? intval($_POST['musicScore']) : 0; 


$sql = "SELECT musicScore FROM musiclog WHERE userID = '$userId' AND musicID = '$musicId'";
$result = $conn->query($sql);

if (!$result || $result->num_rows == 0) {
    echo "newRecord"; // 기록이 없으면 새로운 기록
} else {
    $row = $result->fetch_assoc();
    $dbHighScore = intval($row['musicScore']); 

    if ($newScore > $dbHighScore) {
        echo "newHighScore"; // 새로운 최고 기록
    } else {
        echo "fail"; // 기존 기록 유지
    }
}

$conn->close();
?>