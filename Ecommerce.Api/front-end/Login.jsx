import React, { useState } from 'react';
import { API_URL } from './config';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [otp, setOtp] = useState('');
  const [step, setStep] = useState(1); // 1: Credentials, 2: OTP
  const [isRegistering, setIsRegistering] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const validateEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');
    setIsLoading(true);

    if (step === 1) {
      if (!validateEmail(email)) {
        setError('Please enter a valid email address.');
        setIsLoading(false);
        return;
      }

      const endpoint = isRegistering ? `${API_URL}/register` : `${API_URL}/login`;

      try {
        const response = await fetch(endpoint, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, password }),
        });

        const data = await response.json();

        if (!response.ok) {
          throw new Error(data.message || 'Authentication failed.');
        }

        // If backend requires OTP (e.g. for new registration verification)
        if (data.requiresOtp) {
          setStep(2);
          setMessage(`Authentication code sent to ${email}`);
        } else if (data.token) {
          // Standard login success
          const expirationTime = Date.now() + 2 * 60 * 1000; // 2 minutes from now
          localStorage.setItem('tokenExpiration', expirationTime);
          localStorage.setItem('token', data.token);
          localStorage.setItem('userEmail', email);
          window.location.href = '/';
        } else if (isRegistering) {
          // Registration successful but no token returned (switch to login mode)
          setIsRegistering(false);
          setMessage('Registration successful! Please log in.');
        }
      } catch (err) {
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    } else {
      // Step 2: Verify OTP
      try {
        const response = await fetch(`${API_URL}/verify-otp`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, otp }),
        });

        const data = await response.json();

        if (!response.ok) {
          throw new Error(data.message || 'Invalid code.');
        }

        const expirationTime = Date.now() + 2 * 60 * 1000; // 2 minutes from now
        localStorage.setItem('tokenExpiration', expirationTime);
        localStorage.setItem('token', data.token);
        localStorage.setItem('userEmail', email);
        window.location.href = '/';
      } catch (err) {
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>{step === 1 ? (isRegistering ? 'Register' : 'Login') : 'Enter Code'}</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {message && <p style={{ color: 'green' }}>{message}</p>}

      {step === 1 && (
        <>
          <input type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          <input type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </>
      )}

      {step === 2 && (
        <input type="text" placeholder="Authentication Code" value={otp} onChange={(e) => setOtp(e.target.value)} required />
      )}

      <button type="submit" disabled={isLoading} style={{ opacity: isLoading ? 0.7 : 1 }}>
        {isLoading ? 'Processing...' : (step === 1 ? (isRegistering ? 'Sign Up' : 'Log In') : 'Verify Code')}
      </button>

      {step === 1 && (
        <p style={{ cursor: 'pointer', color: 'blue', marginTop: '10px' }} onClick={() => setIsRegistering(!isRegistering)}>
          {isRegistering ? 'Already have an account? Log In' : 'New user? Register here'}
        </p>
      )}
    </form>
  );
};

export default Login;