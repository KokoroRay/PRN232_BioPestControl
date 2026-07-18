const http = require('http');

const loginData = JSON.stringify({ email: "test999@biopest.com", password: "Password@123" });

const req = http.request({
  hostname: '192.168.1.16', port: 80, path: '/api/auth/login', method: 'POST',
  headers: { 'Content-Type': 'application/json', 'Content-Length': loginData.length }
}, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => {
    try {
        const token = JSON.parse(body).data.token;
        const payload = token.split('.')[1];
        console.log(Buffer.from(payload, 'base64').toString('utf8'));
    } catch(e) {
        console.error("Failed:", body);
    }
  });
});
req.write(loginData);
req.end();
