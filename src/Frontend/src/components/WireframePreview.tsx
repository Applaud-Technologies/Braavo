import DOMPurify from 'dompurify';

interface WireframePreviewProps {
  html: string;
  screenName: string;
}

export default function WireframePreview({ html, screenName }: WireframePreviewProps) {
  const sanitizedHtml = DOMPurify.sanitize(html, {
    ALLOWED_TAGS: ['div', 'span', 'p', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'ul', 'ol', 'li', 'table', 'tr', 'td', 'th', 'thead', 'tbody', 'input', 'button', 'label', 'form', 'header', 'footer', 'nav', 'main', 'section', 'article', 'aside', 'a', 'img', 'br', 'hr'],
    ALLOWED_ATTR: ['class', 'style', 'placeholder', 'type', 'disabled', 'readonly', 'src', 'alt', 'href'],
  });

  return (
    <div className="border border-gray-200 rounded-lg overflow-hidden bg-white">
      <div className="bg-gray-100 px-4 py-2 border-b border-gray-200">
        <h3 className="text-sm font-medium text-gray-700">{screenName}</h3>
      </div>
      <div
        className="p-4 min-h-[400px]"
        dangerouslySetInnerHTML={{ __html: sanitizedHtml }}
      />
    </div>
  );
}
