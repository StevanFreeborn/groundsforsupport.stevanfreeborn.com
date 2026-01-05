import { useState, useRef } from 'react';

type DonationFormProps = {
  onValidSubmit: (data: { name: string; amount: number; message?: string; email?: string }) => void;
  isSubmitting?: boolean;
};

export default function PaymentForm({ onValidSubmit, isSubmitting }: DonationFormProps) {
  const [name, setName] = useState<string>('');
  const [amount, setAmount] = useState<number | ''>('');
  const [message, setMessage] = useState<string>('');
  const [email, setEmail] = useState<string>('');
  const [errors, setErrors] = useState<{
    name?: string;
    amount?: string;
    email?: string;
    message?: string;
  }>({});

  const nameInputRef = useRef<HTMLInputElement>(null);
  const amountInputRef = useRef<HTMLInputElement>(null);
  const messageTextAreaRef = useRef<HTMLTextAreaElement>(null);
  const emailInputRef = useRef<HTMLInputElement>(null);

  function handleNameInput(event: React.ChangeEvent<HTMLInputElement>) {
    setErrors(prevErrors => ({ ...prevErrors, name: undefined }));
    setName(event.target.value);
  }

  function handleAmountInput(event: React.ChangeEvent<HTMLInputElement>) {
    setErrors(prevErrors => ({ ...prevErrors, amount: undefined }));
    const value = parseInt(event.target.value);
    setAmount(isNaN(value) ? '' : value);
  }

  function handleMessageInput(event: React.ChangeEvent<HTMLTextAreaElement>) {
    setErrors(prevErrors => ({ ...prevErrors, message: undefined }));
    setMessage(event.target.value);
  }

  function handleEmailInput(event: React.ChangeEvent<HTMLInputElement>) {
    setErrors(prevErrors => ({ ...prevErrors, email: undefined }));
    setEmail(event.target.value);
  }

  function validateForm() {
    const newErrors: { amount?: string; email?: string; name?: string; message?: string } = {};

    if (name.trim() === '') {
      newErrors.name = 'Please enter your name.';
    }

    if (nameInputRef.current?.validity.tooLong) {
      newErrors.name = 'Name must be 60 characters or less.';
    }

    if (amount === '' || amountInputRef.current?.validity.rangeUnderflow || amount <= 0) {
      newErrors.amount = 'Please enter a valid amount greater than 0.';
    }

    if (message.length > 250) {
      newErrors.message = 'Message must be 250 characters or less.';
    }

    if (email.trim() !== '' && emailInputRef.current?.validity.typeMismatch) {
      newErrors.email = 'Please enter a valid email address.';
    }

    setErrors(newErrors);

    const isValid = Object.keys(newErrors).length === 0;

    if (isValid === false) {
      if (newErrors.name) {
        nameInputRef.current?.focus();
      } else if (newErrors.amount) {
        amountInputRef.current?.focus();
      } else if (newErrors.message) {
        messageTextAreaRef.current?.focus();
      } else if (newErrors.email) {
        emailInputRef.current?.focus();
      }
    }

    return isValid;
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const isValid = validateForm();

    if (isValid === false) {
      return;
    }

    if (amount === '') {
      throw new Error('amount after validation should never be an empty string');
    }

    onValidSubmit({ name, amount, message, email });
  }

  return (
    <form
      onSubmit={handleSubmit}
      noValidate
    >
      <div className='group'>
        <label htmlFor='name'>Name</label>
        <input
          ref={nameInputRef}
          id='name'
          type='text'
          required
          maxLength={60}
          aria-describedby='nameErrorMessage'
          aria-invalid={errors.name ? 'true' : 'false'}
          value={name}
          onInput={handleNameInput}
          placeholder='What do I call you?'
        />
        <span
          id='nameErrorMessage'
          className='error-message'
        >
          {errors.name}
        </span>
      </div>
      <div className='group'>
        <label htmlFor='amount'>Amount</label>
        <input
          ref={amountInputRef}
          id='amount'
          type='number'
          required
          min={1}
          aria-describedby='amountErrorMessage'
          aria-invalid={errors.amount ? 'true' : 'false'}
          value={amount}
          onInput={handleAmountInput}
          placeholder='1'
        />
        <span
          id='amountErrorMessage'
          className='error-message'
        >
          {errors.amount}
        </span>
      </div>
      <div className='group'>
        <label htmlFor='message'>Message</label>
        <textarea
          id='message'
          ref={messageTextAreaRef}
          aria-describedby='messageErrorMessage'
          aria-invalid={errors.message ? 'true' : 'false'}
          value={message}
          maxLength={250}
          onInput={handleMessageInput}
          placeholder='Let me know what I did for you!'
        />
        <span
          id='messageErrorMessage'
          className='error-message'
        >
          {errors.message}
        </span>
      </div>
      <div className='group'>
        <div className='email-label'>
          <label htmlFor='email'>Email </label>
          <span className='detail'>[if you want a receipt]</span>
        </div>
        <input
          ref={emailInputRef}
          id='email'
          type='email'
          aria-describedby='emailErrorMessage'
          aria-invalid={errors.email ? 'true' : 'false'}
          value={email}
          onInput={handleEmailInput}
          placeholder='hello@world.com'
        />
        <span
          id='emailErrorMessage'
          className='error-message'
        >
          {errors.email}
        </span>
      </div>
      <button
        type='submit'
        disabled={isSubmitting}
      >
        Buy
      </button>
    </form>
  );
}
