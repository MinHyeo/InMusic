<?php
    $host="localhost";
    $user="root";
    $password="";
    $db="inmusic";


    $user_id = $_POST['userID'] ?? "76561198365750763";
    $user_name = $_POST['userName'] ?? "swatper";

    /*if($user_id == null || $user_name == null){
        echo json_encode(array("error" => "User ID or User Name cannot be null"));
        exit;
    }*/

    $conn = new mysqli($host,$user,$password,$db);
    if(!$conn){
        die("Connection failed: " . mysqli_connect_error());
    }

    $sql = "SELECT userName From user Where userName = '$user_name' and userID = '$user_id'";

    $result = mysqli_query($conn, $sql); 
    if($result->num_rows > 0){
        while($row = $result->fetch_assoc()){
            if($row["userName"] == $user_name){
                echo "Login successful, welcome " . $row["userName"];
            }
        }
    }
    //없으면 회원 가입
    else if ($result->num_rows == 0){
        $sql = "INSERT INTO user VALUES ('$user_id', '$user_name')";
        $result = mysqli_query($conn, $sql);
        if($result){
            echo "User created successfully, welcome " . $user_name;
        }
        else{
            echo "Error: " . $sql . "<br>" . $conn->error;
            exit;
        }
        
    }
    else{
        echo "Error: " . $sql . "<br>" . $conn->error;
        exit;
    }
    
    /*

    //로그인 성공시 음악 로그 데이터 가져오기
    ob_start(); //출력 버퍼링 시작
    include 'getlog.php'; //getlog.php 실행 (음악 로그 데이터 출력)
    $musicLogsJson = ob_get_clean(); //출력된 JSON 데이터를 변수에 저장

    $musicLogJson = json_decode($musicLogsJson, true);
    echo json_encode(["logs" => $musicLogJson], JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    */
    
    //echo json_encode($musicLogJson, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    $conn->close();
?>