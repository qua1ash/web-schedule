const express = require('express');
const cors = require('cors');

const app = express();
const PORT = 3001;

// Middleware
app.use(cors());
app.use(express.json());

// In-memory storage for homework (заглушка)
const homeworkStorage = new Map();

// GET homework by date and lesson order
app.get('/api/homework/:date/:order', (req, res) => {
  const { date, order } = req.params;
  const key = `${date}_${order}`;
  const homework = homeworkStorage.get(key) || '';

  console.log(`GET homework for date: ${date}, lesson order: ${order}, content: "${homework}"`);

  res.json({
    date,
    order,
    homework,
    success: true
  });
});

// POST homework for a specific date and lesson order
app.post('/api/homework/:date/:order', (req, res) => {
  const { date, order } = req.params;
  const { homework } = req.body;

  if (homework === undefined) {
    return res.status(400).json({
      success: false,
      error: 'Homework content is required'
    });
  }

  const key = `${date}_${order}`;
  homeworkStorage.set(key, homework);

  console.log(`POST homework for date: ${date}, lesson order: ${order}, content: "${homework}"`);

  res.json({
    date,
    order,
    homework,
    success: true,
    message: 'Homework saved successfully'
  });
});

// Health check endpoint
app.get('/api/health', (req, res) => {
  res.json({
    status: 'OK',
    message: 'Schedule backend is running',
    timestamp: new Date().toISOString()
  });
});

// Start server
app.listen(PORT, () => {
  console.log(`Schedule backend server is running on http://localhost:${PORT}`);
  console.log('Available endpoints:');
  console.log(`  GET  /api/homework/:date/:order - Get homework for a specific date and lesson order`);
  console.log(`  POST /api/homework/:date/:order - Save homework for a specific date and lesson order`);
  console.log(`  GET  /api/health - Health check`);
});