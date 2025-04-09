<?php
    $host="localhost";
    $user="root";
    $password="";
    $db="inMusic";

    #DB 연동
    $conn = new mysqli($host,$user,$password,$db);
    if(!$conn){
        die("Connection failed: " . mysqli_connect_error());
    }

    #Steam ID 받아오기
    $steam_id = $_POST['steam_id'] ?? '';
    $music_name = $_POST['music_name'] ?? 'Heya';
    $music_score = $_POST['music_score'] ?? 0;
    $music_combo = $_POST['music_combo'] ?? 0;
    $music_accuracy = $_POST['music_accuracy'] ?? 0.0;
    $music_rate = $_POST['music_rate'] ?? '';

    #노래 제목으로 노래 번호 찾기
    $music_id = mysqli_query($conn, "SELECT musicId FROM music WHERE musicName = '$music_name' LIMIT 1");

    if (!$music_id) {
        die("musicId 조회 실패: " . mysqli_error($conn));
    }
    
    $row = mysqli_fetch_assoc($music_id);
    if (!$row) {
        die("해당 음악을 찾을 수 없습니다.");
    }
    $music_id = $row['musicId'];

    #노래 기록보고 이번 기록과 최고 기록 비교
    $compare = mysqli_query($conn, "SELECT musicScore FROM music_log WHERE (userID = '$steam_id') AND musicId = '$music_id'");

    $row = mysqli_fetch_assoc($compare);
    if (!$row) {
        mysqli_query($conn, "INSERT INTO `music_log`(`userId`, `musicId`, `musicScore`, `musicCombo`, `musicAccuracy`, `musicRate`) VALUES ('$steam_id','$music_id','$music_score','$music_combo','$music_accuracy','$music_rate')");
        return;
    }
    $score = $row['musicScore'];

    if($score < $music_score){
        mysqli_query($conn, "UPDATE `music_log` SET `userId`='$steam_id',`musicId`='$music_id',`musicScore`='$music_score',`musicCombo`='$music_combo',`musicAccuracy`='$music_accuracy',`musicRate`='$music_rate' WHERE 1");
    }
    else{
        mysqli_query($conn, "INSERT INTO `music_log`(`userId`, `musicId`, `musicScore`, `musicCombo`, `musicAccuracy`, `musicRate`) VALUES ('$steam_id','$music_id','$music_score','$music_combo','$music_accuracy','$music_rate')");
    }

    echo "점수 등록 완료";

    mysqli_close($conn);
?>