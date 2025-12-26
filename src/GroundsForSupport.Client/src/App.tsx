import '@/App.css';
import { useRef, useState } from 'react';

function App() {
  const [amount, setAmount] = useState<number | ''>('');
  const [email, setEmail] = useState<string>('');
  const [errors, setErrors] = useState<{ amount?: string; email?: string }>({});

  const amountInputRef = useRef<HTMLInputElement>(null);
  const emailInputRef = useRef<HTMLInputElement>(null);

  function handleAmountInput(event: React.ChangeEvent<HTMLInputElement>) {
    setErrors((prevErrors) => ({ ...prevErrors, amount: undefined }));
    const value = parseInt(event.target.value);
    setAmount(isNaN(value) ? '' : value);
  }

  function handleEmailInput(event: React.ChangeEvent<HTMLInputElement>) {
    setErrors((prevErrors) => ({ ...prevErrors, email: undefined }));
    setEmail(event.target.value);
  }

  function validateForm() {
    const newErrors: { amount?: string; email?: string } = {};

    if (amount === '' || amount <= 0) {
      newErrors.amount = 'Please enter a valid amount greater than 0.';
    }

    if (email.trim() !== '' && emailInputRef.current?.validity.typeMismatch) {
      newErrors.email = 'Please enter a valid email address.';
    }

    setErrors(newErrors);

    const isValid = Object.keys(newErrors).length === 0;

    if (isValid === false) {
      if (newErrors.amount) {
        amountInputRef.current?.focus();
      } else if (newErrors.email) {
        emailInputRef.current?.focus();
      }
    }

    return isValid;
  }

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const isValid = validateForm();

    if (isValid === false) {
      return;
    }

    // TODO: We will stop displaying our form
    // and we will initialize the stripe embedded
    // elements and pass along the amount and email
    alert(`Form is valid! Amount: ${amount}, Email: ${email}`);
  }

  return (
    <>
      <header>
        <img
          src='https://github.com/StevanFreeborn.png'
          alt='Profile picture of Stevan Freeborn'
        />
        <div className='info'>
          <h1>Stevan Freeborn</h1>
          <p>
            I'm a dad of 2 who enjoys drinking coffee, lifting weights, and solving problems with
            code. You really don't need to buy me a coffee.
          </p>
        </div>
      </header>
      <main>
        <form
          onSubmit={handleSubmit}
          noValidate
        >
          <div className='group'>
            <label htmlFor='amount'>Amount</label>
            <input
              ref={amountInputRef}
              id='amount'
              type='number'
              min={1}
              aria-describedby='amountErrorMessage'
              aria-invalid={errors.amount ? 'true' : 'false'}
              value={amount}
              onInput={handleAmountInput}
            />
            <span className='error-message'>{errors.amount}</span>
          </div>
          <div className='group'>
            <label htmlFor='email'>Email</label>
            <input
              ref={emailInputRef}
              id='email'
              type='email'
              aria-describedby='emailErrorMessage'
              aria-invalid={errors.email ? 'true' : 'false'}
              value={email}
              onInput={handleEmailInput}
            />
            <span
              id='emailErrorMessage'
              className='error-message'
            >
              {errors.email}
            </span>
          </div>
          <button type='submit'>Buy</button>
        </form>
      </main>
    </>
  );
}

export default App;
