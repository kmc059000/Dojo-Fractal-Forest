open System
open System.Drawing
open System.Windows.Forms

// Create a form to display the graphics
let width, height = 800, 800
let form = new Form(Width = width, Height = height)
let box = new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill)
let image = new Bitmap(width, height)
let graphics = Graphics.FromImage(image)
//The following line produces higher quality images, 
//at the expense of speed. Uncomment it if you want
//more beautiful images, even if it's slower.
//Thanks to https://twitter.com/AlexKozhemiakin for the tip!
graphics.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.HighQuality
let brush = new SolidBrush(Color.FromArgb(0, 0, 0))

box.Image <- image
form.Controls.Add(box) 

// Compute the endpoint of a line
// starting at x, y, going at a certain angle
// for a certain length. 
let endpoint x y angle length =
    x + length * cos angle,
    y + length * sin angle

let flip x = (float)height - x

// Utility function: draw a line of given width, 
// starting from x, y
// going at a certain angle, for a certain length.
let drawLine (target : Graphics) (brush : Brush) 
             (x : float) (y : float) 
             (angle : float) (length : float) (width : float) =
    let x_end, y_end = endpoint x y angle length
    let origin = new PointF((single)x, (single)(y |> flip))
    let destination = new PointF((single)x_end, (single)(y_end |> flip))
    let pen = new Pen(brush, (single)width)
    target.DrawLine(pen, origin, destination)
    x_end, y_end

let draw x y angle length width = 
    drawLine graphics brush x y angle length width

let pi = Math.PI

let rand = System.Random()

let maxDepth = 15
let startLength = 90.
let startWidth = 20.

let lengthReductionRate = 0.8
let widthReductionRate = 0.7

let angleMidpoint = 0.25
let maxAngle = 0.35
let terminateChance = 0.15

let getAngle x = 
    let r = rand.NextDouble () 
    angleMidpoint + (r * maxAngle)
    

let terminateBranch (depth:int) =
    let fDepth = float depth
    let fMaxDepth = float maxDepth
    let r = rand.NextDouble ()
    let threshold = (fDepth * terminateChance / fMaxDepth)
    r < threshold

let getLength length (depth:int) =
    let fDepth = float depth
    let r = rand.NextDouble() * 0.05
    length * (lengthReductionRate + r)

let rec branch depth x y angle length width =
    if depth = maxDepth then ()
    else if (terminateBranch depth) then ()
    else
        let x', y' = draw x y angle length width
        branch (depth + 1) x' y' (angle - getAngle ()) (getLength length depth) (width * widthReductionRate)
        branch (depth + 1) x' y' (angle + getAngle ()) (getLength length depth) (width * widthReductionRate)

branch 0 400. 150. (pi*(0.5)) startLength startWidth

form.ShowDialog()

(* To do a nice fractal tree, using recursion is
probably a good idea. The following link might
come in handy if you have never used recursion in F#:
http://en.wikibooks.org/wiki/F_Sharp_Programming/Recursion
*)