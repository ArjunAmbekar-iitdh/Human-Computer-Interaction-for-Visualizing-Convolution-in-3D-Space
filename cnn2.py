import torch
import torch.nn as nn
import torch.optim as optim
from torchvision import datasets, transforms

# 1. Define a "Sentis-Friendly" Model
class VisualizableCNN(nn.Module):
    def __init__(self):
        super(VisualizableCNN, self).__init__()
        # We use explicit layers so we can hook into them later
        self.conv1 = nn.Conv2d(1, 8, kernel_size=3, padding=1)
        self.relu1 = nn.ReLU()
        self.pool1 = nn.MaxPool2d(2, 2)
        
        self.conv2 = nn.Conv2d(8, 16, kernel_size=3, padding=1)
        self.relu2 = nn.ReLU()
        self.pool2 = nn.MaxPool2d(2, 2)
        
        self.flatten = nn.Flatten()
        self.fc1 = nn.Linear(16 * 7 * 7, 10)

    def forward(self, x):
        # We name these variables so we know what we are exporting
        x = self.relu1(self.conv1(x)) # Output: 28x28x8
        x = self.pool1(x)             # Output: 14x14x8
        x = self.relu2(self.conv2(x)) # Output: 14x14x16
        x = self.pool2(x)             # Output: 7x7x16
        x = self.flatten(x)
        x = self.fc1(x)
        return x

# 2. Quick Training Setup
transform = transforms.Compose([transforms.ToTensor(), transforms.Normalize((0.1307,), (0.3081,))])
train_loader = torch.utils.data.DataLoader(datasets.MNIST('./data', train=True, download=True, transform=transform), batch_size=64, shuffle=True)

model = VisualizableCNN()
optimizer = optim.Adam(model.parameters(), lr=0.001)
criterion = nn.CrossEntropyLoss()

# 3. Mini-Train (1 Epoch is enough for a demo model)
model.train()
for batch_idx, (data, target) in enumerate(train_loader):
    optimizer.zero_grad()
    output = model(data)
    loss = criterion(output, target)
    loss.backward()
    optimizer.step()
    if batch_idx % 100 == 0:
        print(f"Batch {batch_idx} Loss: {loss.item():.4f}")

# 4. Export to ONNX
model.eval()
dummy_input = torch.randn(1, 1, 28, 28)
torch.onnx.export(model, dummy_input, "mnist_viz.onnx", 
                  input_names=['input'], 
                  output_names=['output'],
                  dynamic_axes={'input': {0: 'batch_size'}, 'output': {0: 'batch_size'}})

print("Model exported as mnist_viz.onnx")