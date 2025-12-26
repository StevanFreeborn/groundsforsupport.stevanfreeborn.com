import { beforeEach, describe, expect, test, vi } from 'vitest';
import App from '@/App.tsx';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import userEvent, { type UserEvent } from '@testing-library/user-event';

describe('profile info', () => {
  test('it should display a profile image', () => {
    const screen = render(<App />);
    
    const profileImage = screen.getByAltText('Profile picture of Stevan Freeborn');

    expect(profileImage).toBeInTheDocument();
  });

  test('it should display name', () => {
    const screen = render(<App />);

    const nameElement = screen.getByRole('heading', { name: 'Stevan Freeborn' });

    expect(nameElement).toBeInTheDocument();
  });
});

describe('form', () => {
  let user: UserEvent;

  beforeEach(() => {
    user = userEvent.setup();
  });

  test('it should have an amount input', () => {
    const screen = render(<App />);

    const amountInput = screen.getByLabelText('Amount');

    expect(amountInput).toBeInTheDocument();
  });

  test('it should have an email input', () => {
    const screen = render(<App />);

    const emailInput = screen.getByLabelText('Email');

    expect(emailInput).toBeInTheDocument();
  });

  test('it should have a button to submit the form', () => {
    const screen = render(<App />);

    const submitButton = screen.getByRole('button', { name: 'Buy' });

    expect(submitButton).toBeInTheDocument();
  });

  test('it should require the amount to be provided', async () => {
    const screen = render(<App />);

    const submitButton = screen.getByRole('button', { name: 'Buy' });

    await user.click(submitButton);

    const errorMessage = screen.getByText('Please enter a valid amount greater than 0.');
    expect(errorMessage).toBeInTheDocument();
  });

  test('it should clear amount error when input changes', async () => {
    const screen = render(<App />);

    const amountInput = screen.getByLabelText('Amount');
    const submitButton = screen.getByRole('button', { name: 'Buy' });

    await user.click(submitButton);

    let errorMessage = screen.queryByText('Please enter a valid amount greater than 0.');
    expect(errorMessage).toBeInTheDocument();

    await userEvent.type(amountInput, '10');

    errorMessage = screen.queryByText('Please enter a valid amount greater than 0.');

    expect(errorMessage).not.toBeInTheDocument();
  });

  test('it should allow form to be submitted without email', async () => {
    const windowSpy = vi.spyOn(window, 'alert').mockImplementation(() => {});

    const screen = render(<App />);

    const amountInput = screen.getByLabelText('Amount');
    const submitButton = screen.getByRole('button', { name: 'Buy' });

    await user.type(amountInput, '10');
    await user.click(submitButton);

    expect(windowSpy).toHaveBeenCalledWith('Form is valid! Amount: 10, Email: ');
  });

  test('it should validate email if provided', async () => {
    const screen = render(<App />);

    const amountInput = screen.getByLabelText('Amount');
    const emailInput = screen.getByLabelText('Email');
    const submitButton = screen.getByRole('button', { name: 'Buy' });

    await user.type(amountInput, '10');
    await user.type(emailInput, 'invalid-email');
    await user.click(submitButton);

    const errorMessage = screen.getByText('Please enter a valid email address.');
    expect(errorMessage).toBeInTheDocument();
  });

  test('it should clear email error when input changes', async () => {
    const screen = render(<App />);

    const amountInput = screen.getByLabelText('Amount');
    const emailInput = screen.getByLabelText('Email');
    const submitButton = screen.getByRole('button', { name: 'Buy' });

    await user.type(amountInput, '10');
    await user.type(emailInput, 'invalid-email');
    await user.click(submitButton);

    let errorMessage = screen.queryByText('Please enter a valid email address.');
    expect(errorMessage).toBeInTheDocument();

    await user.clear(emailInput);
    await user.type(emailInput, 'test@test.com');

    errorMessage = screen.queryByText('Please enter a valid email address.');
    expect(errorMessage).not.toBeInTheDocument();
  });
});
