<?php
    header('Content-Type: application/json; charset=utf-8');

    $host="localhost";
    $user="root";
    $password="";
    $db="inmusic";

    $json_data = $_POST["musics"];;

    $conn = new mysqli($host,$user,$password,$db);
    if(!$conn){
        echo "Connection failed: " . mysqli_connect_error();
        die("Connection failed: " . mysqli_connect_error());
    }

    $json_data = $_POST["musics"]; //Unity에서 보낸 JSON
    $musicList = json_decode($json_data, true); //JSON을 배열로 변환
    //echo json_encode($musicList); //디버깅용

    //응답용 배결
    $response = [];

    foreach ($musicList["musics"] as $music) {
        $musicID = $conn->real_escape_string($music["musicID"]);
        $musicName = $conn->real_escape_string($music["musicName"]);
        $musicArtist = $conn->real_escape_string($music["musicArtist"]);

        //musicID가 이미 존재하는지 체크
        $check_sql = "SELECT musicID FROM music WHERE musicID = '$musicID'";
        $result = $conn->query($check_sql);

        //기존에 없으면 INSERT
        if ($result->num_rows == 0) { 
            $insert_sql = "INSERT INTO music (musicID, musicName, musicArtist) VALUES ('$musicID', '$musicName', '$musicArtist')";
            
            if ($conn->query($insert_sql) === TRUE) {
                $response[] = "$musicID 추가됨";
            }
        }
    }
    
    echo "음악 데이터 동기화 완료\n";
    if($response == null){
        echo "추가된 음악 없음";
    }
    else{
        echo implode("\n", $response);
    }

    $conn->close();
?>