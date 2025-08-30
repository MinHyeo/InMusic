<?php
header("Content-Type: application/json; charset=UTF-8");

// DB 접속 정보
$servername = "localhost";
$dbUsername = "root";
$dbPassword = "";
$dbName     = "inmusic";

// 유니티에서 POST 받은 값
$musicId      = $_POST["musicId"];      // 예: "Klaxon" (또는 유니크한 값)
$musicName    = $_POST["musicName"];    // 예: "Klaxon (Title)"
$musicArtist  = $_POST["musicArtist"];  // 예: "Some Artist"

$conn = new mysqli($servername, $dbUsername, $dbPassword, $dbName);
if ($conn->connect_error) {
    $response = [
        "success" => false,
        "message" => "DB Connection failed: " . $conn->connect_error
    ];
    echo json_encode($response);
    exit();
}

// ON DUPLICATE KEY UPDATE 구문: 이미 있으면 UPDATE, 없으면 INSERT
$sql = "
INSERT INTO Music (musicId, musicName, musicArtist)
VALUES ('$musicId', '$musicName', '$musicArtist')
ON DUPLICATE KEY UPDATE
  musicName='$musicName',
  musicArtist='$musicArtist'
";

if ($conn->query($sql) === TRUE) {
    $response = [
        "success" => true,
        "message" => "Music upsert success"
    ];
} else {
    $response = [
        "success" => false,
        "message" => "Error: " . $conn->error
    ];
}

$conn->close();
echo json_encode($response);
?>
