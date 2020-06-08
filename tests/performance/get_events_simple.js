import { check, group, sleep } from "k6";
import http from "k6/http";
import { Counter, Rate, Trend } from "k6/metrics";


// Options
export let options = {
    stages: [
        { target: 10, duration: "15s" },
        { target: 20, duration: "20s" },
        { target: 30, duration: "15s" },
        { target: 0, duration: "10s" }
    ],
    thresholds: {
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

        sleep(1);
    });
}