# 蓝牙通信软件跨平台改造计划

## 一、项目目标

将当前 React Web 应用改造为支持以下平台的本地离线应用：
- **Windows** - 桌面应用
- **macOS** - 桌面应用
- **Android** - 移动应用

并实现真实蓝牙设备的自动识别、端口识别和地址读取。

---

## 二、技术方案选择

### 方案对比

| 方案 | Windows | macOS | Android | 蓝牙支持 | 推荐度 |
|------|---------|-------|---------|----------|--------|
| **Electron** | ✅ | ✅ | ❌ | noble/electron-bluetooth | ⭐⭐⭐ |
| **React Native** | ✅ | ✅ | ✅ | react-native-bluetooth-classic | ⭐⭐⭐⭐⭐ |
| **Capacitor** | ✅ | ✅ | ✅ | capacitor-bluetooth-classic | ⭐⭐⭐⭐ |
| **Tauri** | ✅ | ✅ | ❌ | tauri-plugin-bluetooth | ⭐⭐⭐ |

### 推荐方案：React Native + Expo

**理由：**
1. 一套代码支持 Windows、macOS、Android 三端
2. 成熟的蓝牙库支持（react-native-bluetooth-classic）
3. 可离线运行，打包为原生应用
4. 社区活跃，文档完善

---

## 三、蓝牙通信方案

### 3.1 蓝牙协议选择

| 协议 | 说明 | 适用场景 |
|------|------|----------|
| **RFCOMM (Classic Bluetooth)** | 传统蓝牙串口协议 | 压电模块通信（推荐） |
| **BLE (Bluetooth Low Energy)** | 低功耗蓝牙 | 传感器数据采集 |

### 3.2 通信参数

| 参数 | 默认值 | 说明 |
|------|--------|------|
| 波特率 | 9600 / 115200 | 可配置 |
| 数据位 | 8 | 固定 |
| 停止位 | 1 | 固定 |
| 校验位 | None | 无校验 |
| 流控制 | None | 无流控 |

### 3.3 数据包格式

```
[包头][通道数][数据1][数据2]...[数据N][包尾][校验和]

示例：
0xAA 0x04 0x12 0x34 0x56 0x78 0x55 CRC
```

---

## 四、实施步骤

### 阶段一：项目重构（预计 2-3 小时）

#### 步骤 1：创建 React Native 项目
```bash
# 使用 Expo 创建项目
npx create-expo-app@latest cardiac-lead-app --template blank-typescript
```

#### 步骤 2：安装蓝牙依赖
```bash
# 蓝牙经典库（用于串口通信）
npm install react-native-bluetooth-classic

# 蓝牙权限管理
npm install expo-permissions

# 串口通信工具
npm install react-native-serialport
```

#### 步骤 3：迁移现有组件
- 迁移蓝牙面板组件
- 迁移指令控制组件
- 迁移波形图组件（使用 react-native-canvas 或 svg）

---

### 阶段二：蓝牙功能实现（预计 3-4 小时）

#### 步骤 4：实现蓝牙设备扫描
```typescript
// 自动扫描已配对设备
async function scanPairedDevices(): Promise<Device[]>

// 扫描新设备
async function discoverDevices(): Promise<Device[]>
```

#### 步骤 5：实现设备连接
```typescript
// 连接设备
async function connectDevice(address: string): Promise<Connection>

// 获取设备信息
interface DeviceInfo {
  name: string;        // 设备名称
  address: string;     // MAC地址
  port: number;        // 端口号
  rssi: number;        // 信号强度
  bonded: boolean;     // 是否已配对
}
```

#### 步骤 6：实现数据通信
```typescript
// 发送数据
async function sendData(data: Buffer): Promise<void>

// 接收数据（流式）
function onDataReceived(callback: (data: Buffer) => void): void
```

---

### 阶段三：平台适配（预计 2-3 小时）

#### 步骤 7：Windows 配置
```json
// app.json
{
  "expo": {
    "platforms": ["windows"],
    "windows": {
      "package": "com.cardiac.lead"
    }
  }
}
```

#### 步骤 8：macOS 配置
```json
{
  "expo": {
    "platforms": ["macos"],
    "macos": {
      "package": "com.cardiac.lead"
    }
  }
}
```

#### 步骤 9：Android 配置
```json
{
  "expo": {
    "platforms": ["android"],
    "android": {
      "package": "com.cardiac.lead",
      "permissions": [
        "android.permission.BLUETOOTH",
        "android.permission.BLUETOOTH_ADMIN",
        "android.permission.BLUETOOTH_CONNECT",
        "android.permission.BLUETOOTH_SCAN"
      ]
    }
  }
}
```

---

### 阶段四：测试与打包（预计 1-2 小时）

#### 步骤 10：平台测试
- Windows: 生成 .exe 安装包
- macOS: 生成 .dmg 安装包
- Android: 生成 .apk 安装包

#### 步骤 11：蓝牙测试
- 测试设备扫描
- 测试设备连接
- 测试数据收发

---

## 五、文件结构（改造后）

```
cardiac-lead-app/
├── src/
│   ├── components/
│   │   ├── BluetoothPanel.tsx      # 蓝牙连接面板
│   │   ├── CommandPanel.tsx        # 指令控制面板
│   │   └── MultiChannelPlot.tsx    # 波形图组件
│   ├── services/
│   │   ├── BluetoothService.ts     # 蓝牙服务
│   │   ├── SerialService.ts        # 串口服务
│   │   └── DataService.ts          # 数据处理服务
│   ├── hooks/
│   │   ├── useBluetooth.ts         # 蓝牙钩子
│   │   └── useSerial.ts            # 串口钩子
│   ├── store/
│   │   └── useAppStore.ts          # 状态管理
│   ├── types/
│   │   └── bluetooth.d.ts          # 类型定义
│   └── screens/
│       └── HomeScreen.tsx          # 主屏幕
├── android/                        # Android 原生配置
├── ios/                            # iOS 原生配置
├── windows/                        # Windows 原生配置
├── macos/                          # macOS 原生配置
├── app.json                        # Expo 配置
└── package.json
```

---

## 六、蓝牙设备信息展示

连接成功后，界面将显示：

```
┌─────────────────────────────────────┐
│ Device Information                  │
├─────────────────────────────────────┤
│ Name:      Pacemaker-Module-001     │
│ Address:   00:1A:7D:DA:71:13        │
│ Port:      RFCOMM Channel 1         │
│ RSSI:      -45 dBm                  │
│ Bonded:    Yes                      │
│ Protocol:  RFCOMM (Serial)          │
│ BaudRate:  115200                   │
└─────────────────────────────────────┘
```

---

## 七、风险与注意事项

### 7.1 权限问题
- **Android 12+** 需要动态申请蓝牙权限
- **Windows** 可能需要管理员权限
- **macOS** 需要在 Info.plist 中声明蓝牙权限

### 7.2 兼容性
- 部分蓝牙模块可能需要特定驱动
- 不同设备的波特率可能不同
- 需要处理连接断开重连逻辑

### 7.3 性能
- 高频数据传输需要优化缓冲区
- 波形绘制需要考虑性能优化

---

## 八、确认清单

请确认以下事项：

- [ ] 是否同意使用 React Native 方案？
- [ ] 目标蓝牙设备类型？（经典蓝牙 RFCOMM / 低功耗蓝牙 BLE）
- [ ] 蓝牙设备通信参数？（波特率、数据格式）
- [ ] 是否需要支持 iOS？（当前计划未包含）
- [ ] 是否有具体的蓝牙设备型号用于测试？

---

## 九、下一步行动

确认计划后，我将：

1. 创建 React Native 项目结构
2. 实现蓝牙服务层
3. 迁移现有 UI 组件
4. 配置多平台打包
5. 进行蓝牙连接测试

---

**请确认以上计划，确认无误后我将开始实施。**
