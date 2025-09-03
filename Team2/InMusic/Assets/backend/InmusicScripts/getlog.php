<?php
    header('Content-Type: application/json');

    $host="localhost";
    $user="root";
    $password="";
    $db="inmusic";

    $logID = $_POST['logID'] ?? '';

    $conn = new mysqli($host,$user,$password,$db);
    if(!$conn){
        echo "Connection failed: " . mysqli_connect_error();
        die("Connection failed: " . mysqli_connect_error());
    }

    if($logID == "all" || $logID == null || $logID == ""){
        $sql = "SELECT logID, musicID,musicScore, musicCombo, musicAccuracy, musicRank FROM musiclog";
    }
    else{
        $sql = "SELECT logID, musicID, musicScore, musicCombo, musicAccuracy, musicRank FROM musiclog where logID = '$logID'";
    }

    $result = mysqli_query($conn, $sql);
    $musicLogs = [];
    if($result){
        while($row = $result->fetch_assoc()){
            $musicLogs[] = [
                "logID" => $row["logID"],
                "musicID" => $row["musicID"],
                "musicScore" => $row["musicScore"],
                "musicAccuracy" => $row["musicAccuracy"],
                "musicCombo" => $row["musicCombo"],
                "musicRank" => $row["musicRank"]
            ];
        }
        echo json_encode(["logs" => $musicLogs], JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    }
    if (!$result) {
        die("쿼리 실행 실패: " . mysqli_error($conn)); // SQL 실행 오류 확인
    }

    $conn->close();
?>