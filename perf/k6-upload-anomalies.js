import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = { vus: 10, duration: '30s', thresholds: { http_req_duration: ['p(95)<200'] } };

export default function () {
  const csv = 'caller_id,recipient,call_date,end_time,duration (s),cost (3 d.p.),reference,currency\n' +
              '1,2,2025-01-01,12:00,60,0.123,'+Math.random()+',USD\n';
  const data = { file: http.file(csv, 'techtest_cdr.csv', 'text/csv') };
  const res = http.post('http://localhost:8080/api/cdr/upload', data);
  check(res, { 'accepted': r => r.status === 202 });
  sleep(1);
}