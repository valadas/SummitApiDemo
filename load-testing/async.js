import http from 'k6/http';
import { sleep } from "k6";

export let options = {
    stages: [
        {
            target: 10,
            duration: "10s",
        },
        {
            target: 100,
            duration: "10s"
        },
        {
            target: 200,
            duration: "10s",
        },
        {
            target: 0,
            duration: "10s",
        },
    ],
}

export default function(){
    http.get("http://asyncsync.eraware.ca/API/Eraware_SummitApiDemo/Test/GetAsync");
    sleep(1);
}