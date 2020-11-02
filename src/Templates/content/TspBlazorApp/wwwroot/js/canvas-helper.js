var _canvas;
var _canvasContext;

function initializeCanvas()
{
    _canvas = document.getElementById('canvas');
    _canvasContext = _canvas.getContext('2d');
    _canvas.width  = window.innerWidth * .8;
    _canvas.height = window.innerHeight * .8;

    return [_canvas.width, _canvas.height];
}

function clearCanvas() 
{
    _canvasContext.clearRect(0, 0, _canvas.width, _canvas.height);
}

function drawRect(x, y, width, height)
{   
    _canvasContext.beginPath();
    _canvasContext.rect(x, y, width, height);
    _canvasContext.stroke();
}

function drawCircle(x, y, radius)
{   
    _canvasContext.fillStyle = "#000000";
    _canvasContext.beginPath();
    _canvasContext.arc(x, y, radius, 0, 2 * Math.PI);
    _canvasContext.fill();
}

function drawLine(x1, y1, x2, y2)
{
    _canvasContext.beginPath();
    _canvasContext.moveTo(x1, y1);
    _canvasContext.lineTo(x2, y2);
    _canvasContext.stroke();
}