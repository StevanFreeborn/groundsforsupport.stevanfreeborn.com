export type Payment = {
  name: string;
  amount: number;
  message?: string;
  createdAtUnix: number;
};

export default function PaymentCard({ payment }: { payment: Payment }) {
  const formattedPayment = payment.amount / 100;

  const date = new Date(payment.createdAtUnix);
  const userLocale = navigator.language || 'en-US';
  const formattedDate = date.toLocaleDateString(userLocale, {
    year: 'numeric',
    month: 'numeric',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
    hour12: true,
  });

  return (
    <div className='payment-card'>
      <div className='header'>
        <div className='left'>
          <img
            src='/espresso.png'
            alt='Avatar'
          />
        </div>
        <div className='right'>
          <div className='info'>
            <div>{payment.name}</div>
            <div className='date'>{formattedDate}</div>
          </div>
          <div className='details'>
            <div className='amount'>${formattedPayment.toFixed(2)}</div>
          </div>
        </div>
      </div>
      {payment.message && <div className='message'>{payment.message}</div>}
    </div>
  );
}
