const http = require('http');

const loginData = JSON.stringify({ email: "admin@biopest.com", password: "Admin@123" });

const options = {
  hostname: 'identity-service.biopest.svc.cluster.local',
  port: 80,
  path: '/api/auth/login',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Content-Length': loginData.length
  }
};

const req = http.request(options, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => {
    const token = JSON.parse(body).data.token;
    
    const cartReq = http.request({
      hostname: 'ordering-service.biopest.svc.cluster.local',
      port: 80,
      path: '/api/cart',
      method: 'GET',
      headers: { 'Authorization': `Bearer ${token}` }
    }, res2 => {
      let b2 = '';
      res2.on('data', d => b2 += d);
      res2.on('end', () => console.log('Cart Status:', res2.statusCode, b2));
    });
    cartReq.end();
  });
});

req.write(loginData);
req.end();
