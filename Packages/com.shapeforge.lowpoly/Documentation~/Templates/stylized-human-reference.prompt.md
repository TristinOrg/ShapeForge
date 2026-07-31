# Stylized Human Reference Extraction

Use this protocol with a vision-capable LLM and the adjacent `stylized-human-reference-1.0.schema.json`.

## Input requirements

- One full-body front image is required. A distinct side image is optional.
- Use an orthographic or long-lens view where possible. Strong perspective makes ratios unreliable.
- The top of the hair, jaw, shoulders, torso sides, hips, knees, and bottom of both feet must be visible.
- Request a clearer image when required landmarks are cropped or occluded; do not guess them.

## Measurement rules

- Treat image-left as zero and image-right as one for `parting`.
- Divide head width, head height, shoulder width, body width, and leg length by full figure height from top of hair to bottom of feet.
- Measure leg length from the hip joint line to the bottom of the feet.
- Divide jaw width by head width and hair width by head width.
- Normalize fringe length from hairline zero to jaw one and sideburn length from temple zero to jaw one.
- Exclude weapons, loose accessories, shadows, background, and empty transparent padding.
- Emit `side` only from a separate side image. Never estimate depth from the front image.

## Output contract

Return exactly one JSON object that conforms to the supplied Schema. Do not add Markdown, comments, prose, confidence text, pixel coordinates, or unsupported properties.

The same compact instruction is available at runtime through `LowPolyStylizedHumanReferencePrompt.Protocol`; use `Create(schemaJson)` to append the authoritative Schema without binding ShapeForge to an AI provider.
