import { check, group, sleep } from "k6";
import http from "k6/http";
import { Counter, Rate, Trend } from "k6/metrics";


// Options
export let options = {
    stages: [
        { target: 100, duration: "30s" },
        { target: 80, duration: "60s" },
        { target: 120, duration: "70s" },
        { target: 0, duration: "20s" }
    ],
    // stages: [
    //     { target: 100, duration: "3s" },
    //     { target: 80, duration: "12s" },
    //     { target: 120, duration: "6s" },
    //     { target: 0, duration: "3s" }
    // ],
    thresholds: {
        "http_req_duration": ["p(95)<500"],
        "http_req_duration": ["p(99)<10"],
        "check_failure_rate": ["rate<0.3"]
    }
};

// Custom metrics
var successfulLogins = new Counter("successful_logins");
var checkFailureRate = new Rate("check_failure_rate");
var timeToFirstByte = new Trend("time_to_first_byte", true);

// User scenario
export default function() {
    group("Get Events", function() {
        let res = http.get("https://historicevents.azurewebsites.net/api/events");

        let checkRes = check(res, {
            "is status 200": (r) => r.status === 200,
            "body is greater than 200 bytes": (r) => r.body.length > 200,
        });

        // Record check failures
        checkFailureRate.add(!checkRes);

        // Record time to first byte and tag it with the URL 
        timeToFirstByte.add(res.timings.waiting, { url: res.url });

        sleep(5);
    });
}