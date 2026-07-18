const http = require('http');

const regData = JSON.stringify({ 
  email: "test999@biopest.com", 
  password: "Password@123", 
  fullName: "Test User",
  role: "Customer"
});

const loginData = JSON.stringify({ email: "test999@biopest.com", password: "Password@123" });

function login() {
  const req = http.request({
    hostname: '192.168.1.16', port: 80, path: '/api/auth/login', method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Content-Length': loginData.length }
  }, res => {
    let body = '';
    res.on('data', d => body += d);
    res.on('end', () => {
      try {
          const token = JSON.parse(body).data.token;
          console.log("Got token.");
          
          const cartReq = http.request({
            hostname: '192.168.1.16', port: 80, path: '/api/cart', method: 'GET',
            headers: { 'Authorization': `Bearer ${token}` }
          }, res2 => {
            let b2 = '';
            res2.on('data', d => b2 += d);
            res2.on('end', () => console.log('Cart Status:', res2.statusCode, '\nBody:', b2));
          });
          cartReq.end();
      } catch(e) {
          console.error("Failed to parse login response:", body);
      }
    });
  });
  req.write(loginData);
  req.end();
}

const req = http.request({
  hostname: '192.168.1.16', port: 80, path: '/api/auth/register', method: 'POST',
  headers: { 'Content-Type': 'application/json', 'Content-Length': regData.length }
}, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => {
    console.log("Register response:", body);
    login();
  });
});
req.write(regData);
req.end();
