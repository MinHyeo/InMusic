<?php
header("Content-Type: application/json; charset=UTF-8");

$servername = "localhost";
$username   = "root";
$password   = "";
$dbName     = "inmusic";

$conn = new mysqli($servername, $username, $password, $dbName);
if ($conn->connect_error) {
    echo json_encode([
        "success" => false,
        "message" => "DB connection failed: " . $conn->connect_error
    ]);
    exit();
}

// 모든 곡을 SELECT
$sql = "SELECT musicId, musicName, musicArtist FROM Music";
$result = $conn->query($sql);

if (!$result) {
    echo json_encode([
        "success" => false,
        "message" => "Query error: " . $conn->error
    ]);
    $conn->close();
    exit();
}

$songs = [];
while ($row = $result->fetch_assoc()) {
    $songs[] = [
        "musicId"     => $row["musicId"],
        "musicName"   => $row["musicName"],
        "musicArtist" => $row["musicArtist"]
    ];
}

$conn->close();
echo json_encode([
    "success" => true,
    "songs"   => $songs
]);
?>
