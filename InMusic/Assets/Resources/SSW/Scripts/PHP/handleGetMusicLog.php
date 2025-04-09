<?php
header("Content-Type: application/json; charset=UTF-8");

$servername   = "localhost";
$dbUsername   = "root";
$dbPassword   = "";
$dbName       = "inmusic";

$userId = $_POST["userId"];  // 유니티에서 전달한 스팀 유저 ID

$conn = new mysqli($servername, $dbUsername, $dbPassword, $dbName);
if ($conn->connect_error) {
    echo json_encode([
        "success" => false,
        "message" => "DB connection failed: " . $conn->connect_error
    ]);
    exit();
}

$sql = "SELECT musicId, musicScore, musicCombo, musicAccuracy, musicRank 
        FROM musiclog 
        WHERE userId = '$userId'";
$result = $conn->query($sql);

if(!$result) {
    echo json_encode([
        "success" => false,
        "message" => "Query error: " . $conn->error
    ]);
    $conn->close();
    exit();
}

$logs = [];
while ($row = $result->fetch_assoc()) {
    $logs[] = $row;
}

$conn->close();
echo json_encode([
    "success" => true,
    "logs"    => $logs
]);
?>
