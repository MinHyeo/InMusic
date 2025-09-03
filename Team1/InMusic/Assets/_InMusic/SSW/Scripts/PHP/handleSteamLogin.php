<?php
// JSON으로 응답하기 위해 헤더 지정 (선택)
header("Content-Type: application/json; charset=UTF-8");

// DB 접속 정보
$servername = "localhost";
$server_username = "root";
$server_password = "";
$dbName = "inmusic";


$userId   = $_POST["userId"];
$userName = $_POST["userName"];

$conn = new mysqli($servername, $server_username, $server_password, $dbName);

// 연결 에러 처리
if ($conn->connect_error) {
    $response = [
        "success" => false,
        "message" => "DB Connection failed: " . $conn->connect_error
    ];
    echo json_encode($response);
    exit();
}

// 유저 존재 여부 확인
$sqlSelect = "SELECT * FROM user WHERE userId = '$userId'";
$result = $conn->query($sqlSelect);

if (!$result) {
    // SELECT 쿼리 오류
    $response = [
        "success" => false,
        "message" => "Query Error: " . $conn->error
    ];
    echo json_encode($response);
    exit();
}

// 이미 있으면 userName만 UPDATE
if ($result->num_rows > 0) {
    $sqlUpdate = "UPDATE user SET userName = '$userName' WHERE userId = '$userId'";
    $updateResult = $conn->query($sqlUpdate);

    if (!$updateResult) {
        // UPDATE 쿼리 실패
        $response = [
            "success" => false,
            "message" => "Error updating userName: " . $conn->error
        ];
    } else {
        // UPDATE 성공
        $response = [
            "success" => true,
            "newUser" => false,
            "message" => "UserName updated successfully"
        ];
    }
} else {
    // 없으면 새 유저로 INSERT
    $sqlInsert = "INSERT INTO user (userId, userName) VALUES ('$userId', '$userName')";
    $insertResult = $conn->query($sqlInsert);

    if (!$insertResult) {
        // INSERT 실패
        $response = [
            "success" => false,
            "message" => "Error inserting new user: " . $conn->error
        ];
    } else {
        // INSERT 성공
        $response = [
            "success" => true,
            "newUser" => true,
            "message" => "User inserted successfully"
        ];
    }
}

// DB 연결 종료
$conn->close();

// 결과를 JSON으로 출력
echo json_encode($response);
?>
